using FluentValidation;
using Karpinski_XY_Server.Dtos.Exhibition;

namespace Karpinski_XY_Server.Infrastructure.Validators
{
    public class ExhibitionDtoValidator : AbstractValidator<ExhibitionDto>
    {
        public ExhibitionDtoValidator()
        {
            RuleFor(contact => contact.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(contact => contact.Location)
                .NotEmpty()
                .WithMessage("Location is required.");
        }
    }
}
