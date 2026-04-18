using MermaidFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace MermaidFlow.Infrastructure.Mermaid;

/// <summary>
/// Renders Mermaid diagrams to SVG using a pooled headless Chromium browser via Playwright.
/// </summary>
public sealed class PlaywrightMermaidRenderer : IMermaidRenderer
{
    private readonly PlaywrightPagePool _pool;
    private readonly MermaidRendererOptions _options;

    public PlaywrightMermaidRenderer(PlaywrightPagePool pool, IOptions<MermaidRendererOptions> options)
    {
        _pool = pool;
        _options = options.Value;
    }

    public async Task<string> RenderAsync(string mermaidCode, string theme)
    {
        var page = await _pool.BorrowAsync();

        try
        {
            var escaped = EscapeForJs(mermaidCode);

            await page.EvaluateAsync($$"""
                (() => {
                    mermaid.initialize({ startOnLoad: false, theme: '{{theme}}', securityLevel: 'strict' });
                    document.body.innerHTML = '<div id="output"></div><div id="error"></div>';
                })();
                """);

            await page.EvaluateAsync($$"""
                (async () => {
                    try {
                        const { svg } = await mermaid.render('mermaid-diagram', `{{escaped}}`);
                        document.getElementById('output').innerHTML = svg;
                        window.__renderDone = true;
                        window.__renderError = '';
                    } catch (e) {
                        window.__renderDone = true;
                        window.__renderError = e.message || String(e);
                    }
                })();
                """);

            await page.WaitForFunctionAsync(
                "() => window.__renderDone === true",
                null,
                new PageWaitForFunctionOptions { Timeout = _options.RenderTimeoutMs });

            var errorText = await page.EvaluateAsync<string>("window.__renderError || ''");

            if (!string.IsNullOrWhiteSpace(errorText))
            {
                throw new InvalidOperationException($"Mermaid rendering failed: {errorText}");
            }

            var svg = await page.EvalOnSelectorAsync<string>(
                "#output svg",
                "el => el.outerHTML");

            if (string.IsNullOrWhiteSpace(svg))
            {
                throw new InvalidOperationException("Mermaid rendering produced no SVG output.");
            }

            return SvgCleaner.Clean(svg);
        }
        finally
        {
            await CleanUpPageAsync(page);
            await _pool.ReturnAsync(page);
        }
    }

    public async Task<MermaidValidationResult> ValidateAsync(string mermaidCode)
    {
        var page = await _pool.BorrowAsync();

        try
        {
            var escaped = EscapeForJs(mermaidCode);

            await page.EvaluateAsync("""
                (() => {
                    mermaid.initialize({ startOnLoad: false, securityLevel: 'strict' });
                    window.__validationDone = false;
                    window.__isValid = false;
                    window.__errorMessage = '';
                })();
                """);

            await page.EvaluateAsync($$"""
                (async () => {
                    try {
                        await mermaid.parse(`{{escaped}}`);
                        window.__isValid = true;
                    } catch (e) {
                        window.__isValid = false;
                        window.__errorMessage = e.message || String(e);
                    } finally {
                        window.__validationDone = true;
                    }
                })();
                """);

            await page.WaitForFunctionAsync(
                "() => window.__validationDone === true",
                null,
                new PageWaitForFunctionOptions { Timeout = _options.RenderTimeoutMs });

            var isValid = await page.EvaluateAsync<bool>("window.__isValid");
            var errorMessage = await page.EvaluateAsync<string>("window.__errorMessage || ''");

            return new MermaidValidationResult(
                isValid,
                isValid ? null : errorMessage);
        }
        finally
        {
            await CleanUpPageAsync(page);
            await _pool.ReturnAsync(page);
        }
    }

    public async Task<byte[]> RenderToPngAsync(string mermaidCode, string theme)
    {
        var page = await _pool.BorrowAsync();

        try
        {
            var escaped = EscapeForJs(mermaidCode);

            await page.EvaluateAsync($$"""
                (() => {
                    mermaid.initialize({ startOnLoad: false, theme: '{{theme}}', securityLevel: 'strict' });
                    document.body.innerHTML = '<div id="output"></div>';
                })();
                """);

            await page.EvaluateAsync($$"""
                (async () => {
                    try {
                        const { svg } = await mermaid.render('mermaid-diagram-png', `{{escaped}}`);
                        document.getElementById('output').innerHTML = svg;
                        window.__renderDone = true;
                        window.__renderError = '';
                    } catch (e) {
                        window.__renderDone = true;
                        window.__renderError = e.message || String(e);
                    }
                })();
                """);

            await page.WaitForFunctionAsync(
                "() => window.__renderDone === true",
                null,
                new PageWaitForFunctionOptions { Timeout = _options.RenderTimeoutMs });

            var errorText = await page.EvaluateAsync<string>("window.__renderError || ''");

            if (!string.IsNullOrWhiteSpace(errorText))
            {
                throw new InvalidOperationException($"Mermaid rendering failed: {errorText}");
            }

            var element = await page.QuerySelectorAsync("#output svg")
                ?? throw new InvalidOperationException("Mermaid rendering produced no SVG output.");

            return await element.ScreenshotAsync(new ElementHandleScreenshotOptions
            {
                Type = ScreenshotType.Png,
            });
        }
        finally
        {
            await CleanUpPageAsync(page);
            await _pool.ReturnAsync(page);
        }
    }

    private static async Task CleanUpPageAsync(IPage page)
    {
        await page.EvaluateAsync("""
            (() => {
                window.__renderDone = undefined;
                window.__renderError = undefined;
                window.__validationDone = undefined;
                window.__isValid = undefined;
                window.__errorMessage = undefined;
                document.body.innerHTML = '';
            })();
            """);
    }

    private static string EscapeForJs(string input)
        => input
            .Replace("\\", "\\\\")
            .Replace("`", "\\`")
            .Replace("$", "\\$");
}
