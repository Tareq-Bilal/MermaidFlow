using Microsoft.AspNetCore.Mvc.Formatters;

namespace MermaidFlow.Api.Formatters;

public class TextPlainInputFormatter : InputFormatter
{
    public TextPlainInputFormatter()
    {
        SupportedMediaTypes.Add("text/plain");
    }

    protected override bool CanReadType(Type type) => type == typeof(string);

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        using var reader = new StreamReader(context.HttpContext.Request.Body, leaveOpen: true);
        var text = await reader.ReadToEndAsync();
        return await InputFormatterResult.SuccessAsync(text);
    }
}
