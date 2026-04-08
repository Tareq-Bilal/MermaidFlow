using MermaidFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;

namespace MermaidFlow.Infrastructure.Mermaid;

public class PuppeteerSharpMermaidRenderer : IMermaidRenderer, IAsyncDisposable
{
    private readonly ILogger<PuppeteerSharpMermaidRenderer> _logger;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _browserLock = new(1, 1);

    public PuppeteerSharpMermaidRenderer(ILogger<PuppeteerSharpMermaidRenderer> logger)
    {
        _logger = logger;
    }

    public async Task<string> RenderAsync(string mermaidCode, string theme)
    {
        var browser = await GetBrowserAsync();
        await using var page = await browser.NewPageAsync();

        var html = BuildHtml(mermaidCode, theme);
        await page.SetContentAsync(html);

        // Wait for Mermaid to finish rendering the SVG
        var svg = await page.WaitForSelectorAsync("#output svg", new WaitForSelectorOptions
        {
            Timeout = 10_000,
        });

        if (svg is null)
        {
            // Check if Mermaid reported an error
            var errorText = await page.EvaluateExpressionAsync<string>(
                "document.getElementById('error')?.textContent || ''");

            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(errorText)
                    ? "Mermaid rendering timed out — no SVG produced."
                    : $"Mermaid rendering failed: {errorText}");
        }

        var svgContent = await page.EvaluateExpressionAsync<string>(
            "document.getElementById('output').innerHTML");

        return svgContent;
    }

    public async Task<MermaidValidationResult> ValidateAsync(string mermaidCode)
    {
        var browser = await GetBrowserAsync();
        await using var page = await browser.NewPageAsync();

        var html = BuildValidationHtml(mermaidCode);
        await page.SetContentAsync(html);

        // Wait for validation script to complete
        await page.WaitForExpressionAsync(
            "window.__validationDone === true",
            new WaitForFunctionOptions { Timeout = 10_000 });

        var isValid = await page.EvaluateExpressionAsync<bool>("window.__isValid");
        var errorMessage = await page.EvaluateExpressionAsync<string>("window.__errorMessage || ''");

        return new MermaidValidationResult(
            isValid,
            isValid ? null : errorMessage);
    }

    private async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser is { IsClosed: false })
            return _browser;

        await _browserLock.WaitAsync();
        try
        {
            if (_browser is { IsClosed: false })
                return _browser;

            _logger.LogInformation("Downloading Chromium browser for Mermaid rendering...");
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            _logger.LogInformation("Launching headless Chromium browser...");
            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = ["--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage"],
            });

            return _browser;
        }
        finally
        {
            _browserLock.Release();
        }
    }

    private static string BuildHtml(string mermaidCode, string theme)
    {
        var escapedCode = EscapeForJavaScript(mermaidCode);

        return $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <script src="https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.min.js"></script>
            </head>
            <body>
                <div id="output"></div>
                <div id="error"></div>
                <script>
                    mermaid.initialize({
                        startOnLoad: false,
                        theme: '{{theme}}',
                        securityLevel: 'strict'
                    });

                    (async () => {
                        try {
                            const { svg } = await mermaid.render('mermaid-diagram', `{{escapedCode}}`);
                            document.getElementById('output').innerHTML = svg;
                        } catch (e) {
                            document.getElementById('error').textContent = e.message || String(e);
                        }
                    })();
                </script>
            </body>
            </html>
            """;
    }

    private static string BuildValidationHtml(string mermaidCode)
    {
        var escapedCode = EscapeForJavaScript(mermaidCode);

        return $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <script src="https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.min.js"></script>
            </head>
            <body>
                <script>
                    window.__validationDone = false;
                    window.__isValid = false;
                    window.__errorMessage = '';

                    mermaid.initialize({
                        startOnLoad: false,
                        securityLevel: 'strict'
                    });

                    (async () => {
                        try {
                            await mermaid.parse(`{{escapedCode}}`);
                            window.__isValid = true;
                        } catch (e) {
                            window.__isValid = false;
                            window.__errorMessage = e.message || String(e);
                        } finally {
                            window.__validationDone = true;
                        }
                    })();
                </script>
            </body>
            </html>
            """;
    }

    private static string EscapeForJavaScript(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("`", "\\`")
            .Replace("$", "\\$");
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
            _browser.Dispose();
        }

        _browserLock.Dispose();
    }
}
