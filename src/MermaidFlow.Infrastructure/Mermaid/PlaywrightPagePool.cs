using System.Threading.Channels;
using Microsoft.Playwright;

namespace MermaidFlow.Infrastructure.Mermaid;

/// <summary>
/// Manages a fixed pool of pre-warmed Playwright pages with Mermaid.js already loaded.
/// Singleton lifetime — one browser, N pages shared across all requests.
/// </summary>
public sealed class PlaywrightPagePool : IAsyncDisposable
{
    private readonly Channel<IPage> _channel;
    private readonly IPlaywright _playwright;
    private readonly IBrowser _browser;
    private readonly MermaidRendererOptions _options;

    private PlaywrightPagePool(
        IPlaywright playwright,
        IBrowser browser,
        Channel<IPage> channel,
        MermaidRendererOptions options)
    {
        _playwright = playwright;
        _browser = browser;
        _channel = channel;
        _options = options;
    }

    /// <summary>
    /// Factory method — must be called once at startup to warm up the pool.
    /// </summary>
    public static async Task<PlaywrightPagePool> CreateAsync(MermaidRendererOptions options)
    {
        var playwright = await Playwright.CreateAsync();

        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args =
            [
                "--no-sandbox",
                "--disable-setuid-sandbox",
                "--disable-dev-shm-usage",
                "--disable-gpu"
            ]
        });

        var channel = Channel.CreateBounded<IPage>(options.PoolSize);

        for (int i = 0; i < options.PoolSize; i++)
        {
            var page = await browser.NewPageAsync();
            await WarmUpPageAsync(page, options.MermaidJsUrl);
            await channel.Writer.WriteAsync(page);
        }

        return new PlaywrightPagePool(playwright, browser, channel, options);
    }

    /// <summary>Borrow a page from the pool. Caller MUST call ReturnAsync after use.</summary>
    public async Task<IPage> BorrowAsync(CancellationToken ct = default)
        => await _channel.Reader.ReadAsync(ct);

    /// <summary>Return a page back to the pool after use.</summary>
    public async Task ReturnAsync(IPage page)
        => await _channel.Writer.WriteAsync(page);

    private static async Task WarmUpPageAsync(IPage page, string mermaidJsUrl)
    {
        var warmHtml = $$"""
            <html>
            <head>
              <script src="{{mermaidJsUrl}}"></script>
              <script>mermaid.initialize({ startOnLoad: false, securityLevel: 'strict' });</script>
            </head>
            <body></body>
            </html>
            """;

        await page.SetContentAsync(warmHtml, new PageSetContentOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });
    }

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }
}
