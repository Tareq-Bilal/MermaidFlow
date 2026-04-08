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

        // Wait for the rebuild to complete or an error to occur
        await page.WaitForExpressionAsync(
            "window.__cleanSvg !== undefined || document.getElementById('error').textContent !== ''",
            new WaitForFunctionOptions { Timeout = 15_000 });

        var errorText = await page.EvaluateExpressionAsync<string>(
            "document.getElementById('error')?.textContent || ''");

        if (!string.IsNullOrWhiteSpace(errorText))
        {
            throw new InvalidOperationException($"Mermaid rendering failed: {errorText}");
        }

        var svgContent = await page.EvaluateExpressionAsync<string>("window.__cleanSvg");

        if (string.IsNullOrWhiteSpace(svgContent))
        {
            throw new InvalidOperationException("Mermaid rendering produced no SVG output.");
        }

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

                    function rgbToHex(rgb) {
                        if (!rgb || rgb === 'none' || rgb === 'transparent') return 'none';
                        if (rgb.startsWith('#')) return rgb;
                        const m = rgb.match(/rgba?\((\d+),\s*(\d+),\s*(\d+)/);
                        if (!m) return '#333';
                        return '#' + [m[1], m[2], m[3]].map(x => parseInt(x).toString(16).padStart(2, '0')).join('');
                    }

                    function escapeXml(s) {
                        return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
                    }

                    function r(v) { return Math.round(v * 10) / 10; }

                    function rebuildSvg(svgEl) {
                        // --- Coordinate helpers ---
                        function getSvgCoords(el) {
                            const bbox = el.getBBox();
                            const ctm = el.getCTM();
                            if (!ctm) return { x: bbox.x, y: bbox.y, width: bbox.width, height: bbox.height };

                            const p1 = svgEl.createSVGPoint();
                            p1.x = bbox.x; p1.y = bbox.y;
                            const tl = p1.matrixTransform(ctm);

                            const p2 = svgEl.createSVGPoint();
                            p2.x = bbox.x + bbox.width; p2.y = bbox.y + bbox.height;
                            const br = p2.matrixTransform(ctm);

                            return {
                                x: Math.min(tl.x, br.x),
                                y: Math.min(tl.y, br.y),
                                width: Math.abs(br.x - tl.x),
                                height: Math.abs(br.y - tl.y)
                            };
                        }

                        function transformPoint(x, y, ctm) {
                            const p = svgEl.createSVGPoint();
                            p.x = x; p.y = y;
                            return p.matrixTransform(ctm);
                        }

                        const viewBox = svgEl.getAttribute('viewBox');

                        // === EXTRACT NODES ===
                        const nodes = [];
                        svgEl.querySelectorAll('g[class*="node"]').forEach(g => {
                            const shapeEl = g.querySelector('rect, polygon, circle, ellipse');
                            if (!shapeEl) return;

                            const coords = getSvgCoords(shapeEl);
                            const text = g.textContent.trim();
                            if (!text) return;

                            const computed = window.getComputedStyle(shapeEl);
                            const fill = rgbToHex(computed.fill);
                            const stroke = rgbToHex(computed.stroke);
                            const shape = shapeEl.tagName.toLowerCase();

                            nodes.push({
                                x: r(coords.x), y: r(coords.y),
                                width: r(coords.width), height: r(coords.height),
                                cx: r(coords.x + coords.width / 2),
                                cy: r(coords.y + coords.height / 2),
                                text, shape, fill, stroke
                            });
                        });

                        // === EXTRACT EDGES ===
                        const edges = [];
                        const edgePaths = new Set();

                        // Find edge paths by class
                        svgEl.querySelectorAll('path[class*="flowchart-link"], path[class*="edge-pattern"]').forEach(p => edgePaths.add(p));
                        // Paths inside edge groups
                        svgEl.querySelectorAll('g[class*="edge"] path').forEach(p => edgePaths.add(p));
                        // Paths with marker-end that aren't inside node groups
                        svgEl.querySelectorAll('path[marker-end]').forEach(p => {
                            if (!p.closest('g[class*="node"]')) edgePaths.add(p);
                        });

                        edgePaths.forEach(path => {
                            const d = path.getAttribute('d');
                            if (!d || d.length < 5) return;

                            const ctm = path.getCTM();

                            // Extract all coordinate pairs from the path data
                            const allCoords = [...d.matchAll(/([\d.e+-]+)[,\s]+([\d.e+-]+)/g)];
                            if (allCoords.length < 2) return;

                            const first = allCoords[0];
                            const last = allCoords[allCoords.length - 1];

                            let x1 = parseFloat(first[1]), y1 = parseFloat(first[2]);
                            let x2 = parseFloat(last[1]), y2 = parseFloat(last[2]);

                            if (ctm) {
                                const tp1 = transformPoint(x1, y1, ctm);
                                const tp2 = transformPoint(x2, y2, ctm);
                                x1 = tp1.x; y1 = tp1.y;
                                x2 = tp2.x; y2 = tp2.y;
                            }

                            const computed = window.getComputedStyle(path);
                            const stroke = rgbToHex(computed.stroke);

                            edges.push({
                                x1: r(x1), y1: r(y1),
                                x2: r(x2), y2: r(y2),
                                stroke: stroke !== 'none' ? stroke : '#333'
                            });
                        });

                        // === EXTRACT EDGE LABELS ===
                        const edgeLabels = [];
                        svgEl.querySelectorAll('[class*="edgeLabel"]').forEach(g => {
                            const text = g.textContent.trim();
                            if (!text) return;
                            const coords = getSvgCoords(g);
                            edgeLabels.push({
                                x: r(coords.x + coords.width / 2),
                                y: r(coords.y + coords.height / 2),
                                text
                            });
                        });

                        // === BUILD CLEAN SVG ===
                        let out = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="${viewBox}" width="100%">\n\n`;
                        out += `  <defs>\n`;
                        out += `    <marker id="arrow" viewBox="0 0 10 10" refX="5" refY="5"\n`;
                        out += `            markerWidth="6" markerHeight="6" orient="auto">\n`;
                        out += `      <path d="M 0 0 L 10 5 L 0 10 z" fill="#333"/>\n`;
                        out += `    </marker>\n`;
                        out += `  </defs>\n\n`;

                        // Render nodes
                        nodes.forEach(n => {
                            if (n.shape === 'polygon') {
                                // Diamond
                                out += `  <polygon points="${n.cx},${n.y} ${n.x + n.width},${n.cy} ${n.cx},${n.y + n.height} ${n.x},${n.cy}"\n`;
                                out += `          fill="${n.fill}" stroke="${n.stroke}"/>\n`;
                            } else if (n.shape === 'circle' || n.shape === 'ellipse') {
                                out += `  <ellipse cx="${n.cx}" cy="${n.cy}" rx="${r(n.width / 2)}" ry="${r(n.height / 2)}"\n`;
                                out += `           fill="${n.fill}" stroke="${n.stroke}"/>\n`;
                            } else {
                                out += `  <rect x="${n.x}" y="${n.y}" width="${n.width}" height="${n.height}"\n`;
                                out += `        fill="${n.fill}" stroke="${n.stroke}"/>\n`;
                            }
                            out += `  <text x="${n.cx}" y="${n.cy}"\n`;
                            out += `        text-anchor="middle"\n`;
                            out += `        dominant-baseline="middle">${escapeXml(n.text)}</text>\n\n`;
                        });

                        // Render edges
                        edges.forEach(e => {
                            out += `  <line x1="${e.x1}" y1="${e.y1}" x2="${e.x2}" y2="${e.y2}"\n`;
                            out += `        stroke="${e.stroke}"\n`;
                            out += `        marker-end="url(#arrow)"/>\n\n`;
                        });

                        // Render edge labels
                        edgeLabels.forEach(l => {
                            out += `  <text x="${l.x}" y="${l.y}"\n`;
                            out += `        text-anchor="middle"\n`;
                            out += `        dominant-baseline="middle"\n`;
                            out += `        font-size="12">${escapeXml(l.text)}</text>\n\n`;
                        });

                        out += `</svg>`;
                        return out;
                    }

                    (async () => {
                        try {
                            const { svg } = await mermaid.render('mermaid-diagram', `{{escapedCode}}`);
                            document.getElementById('output').innerHTML = svg;
                            const svgEl = document.querySelector('#output svg');
                            if (svgEl) {
                                window.__cleanSvg = rebuildSvg(svgEl);
                            }
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
