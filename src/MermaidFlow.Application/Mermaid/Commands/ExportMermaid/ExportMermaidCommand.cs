using ErrorOr;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Commands.ExportMermaid;

public record ExportMermaidCommand(
    string Code,
    string Theme,
    string Format) : IRequest<ErrorOr<ExportMermaidResult>>;

public record ExportMermaidResult(byte[] Data, string ContentType, string FileName);
