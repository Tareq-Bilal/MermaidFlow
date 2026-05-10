using ErrorOr;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Queries.GetTheme;

public record GetThemeQuery(Guid Id) : IRequest<ErrorOr<Theme>>;
