using FluentValidation;

namespace MermaidFlow.Application.Users.Commands.UpdateUserEmail;

public class UpdateUserEmailCommandValidator : AbstractValidator<UpdateUserEmailCommand>
{
    public UpdateUserEmailCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
