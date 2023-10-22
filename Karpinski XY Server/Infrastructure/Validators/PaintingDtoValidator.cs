using FluentValidation;
using Karpinski_XY_Server.Features.Paintings.Models;

public class PaintingDtoValidator : AbstractValidator<PaintingDto>
{
    public PaintingDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name should not exceed 100 characters.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(p => p.Dimensions)
            .NotEmpty().WithMessage("Dimensions are required.");

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(p => p.PaintingPictures)
            .Must(list => list != null && list.Any()).WithMessage("At least one painting picture must be provided.");

    }
}
