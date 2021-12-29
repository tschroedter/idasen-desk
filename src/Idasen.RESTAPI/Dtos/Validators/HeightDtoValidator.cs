using FluentValidation ;
using JetBrains.Annotations ;

namespace Idasen.RESTAPI.Dtos.Validators
{
    [ UsedImplicitly ]
    public class HeightDtoValidator
        : AbstractValidator < HeightDto >
    {
        public HeightDtoValidator ( )
        {
            RuleFor ( m => m.Height ).Must ( x => x >= MinHeight && x <= MaxHeight )
                                     .WithMessage ( $"Height must be between {MinHeight} and {MaxHeight}" ) ;
        }

        private const uint MinHeight = 6000u ;
        private const uint MaxHeight = 12000u ;
    }
}