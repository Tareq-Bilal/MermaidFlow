using MermaidFlow.Application.Auth;
using MermaidFlow.Domain.Users;

namespace MermaidFlow.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    AuthResult GenerateToken(User user);
}
