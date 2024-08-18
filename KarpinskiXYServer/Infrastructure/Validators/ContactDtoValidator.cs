using FluentValidation;
using Karpinski_XY_Server.Dtos;

namespace Karpinski_XY_Server.Infrastructure.Validators
{
    public class ContactDtoValidator : AbstractValidator<ContactDto>
    {
        public ContactDtoValidator()
        {
            RuleFor(contact => contact.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(contact => contact.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(contact => contact.Content)
                .NotEmpty()
                .WithMessage("Content is required.");
        }
    }
}
