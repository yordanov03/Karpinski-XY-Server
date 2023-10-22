using FluentValidation;
using Karpinski_XY_Server.Features.Paintings.Models;

namespace Karpinski_XY_Server.Validators
{
    public class PaintingPictureDtoValidator : AbstractValidator<PaintingPictureDto>
    {
        public PaintingPictureDtoValidator()
        {
            RuleFor(p => p.ImageUrl)
                .NotEmpty().WithMessage("ImageUrl is required.");
        }
    }
}
