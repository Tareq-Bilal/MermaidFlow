using ErrorOr;
using MermaidFlow.Domain.Mermaid;
using MediatR;

namespace MermaidFlow.Application.Mermaid.Queries.GetThemes;

public record GetThemesQuery() : IRequest<ErrorOr<List<Theme>>>;
