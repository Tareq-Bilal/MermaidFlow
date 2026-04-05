using FluentValidation;

namespace MermaidFlow.Application.Users.Commands.UpdateUserDisplayName;

public class UpdateUserDisplayNameCommandValidator : AbstractValidator<UpdateUserDisplayNameCommand>
{
    public UpdateUserDisplayNameCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
