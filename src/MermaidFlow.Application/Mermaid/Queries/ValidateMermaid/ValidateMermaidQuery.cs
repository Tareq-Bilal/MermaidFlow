using ErrorOr;
using MermaidFlow.Application.Common.Interfaces;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Queries.ValidateMermaid;

public record ValidateMermaidQuery(string Code) : IRequest<ErrorOr<MermaidValidationResult>>;
