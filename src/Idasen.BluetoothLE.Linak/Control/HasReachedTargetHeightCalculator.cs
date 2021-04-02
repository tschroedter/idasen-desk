using System ;
using System.Text.Json ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Control
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class HasReachedTargetHeightCalculator
        : IHasReachedTargetHeightCalculator
    {
        public HasReachedTargetHeightCalculator ( [ NotNull ] ILogger logger )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;

            _logger = logger ;
        }

        /// <inheritdoc />
        public int MovementUntilStop { get ; set ; }

        /// <inheritdoc />
        public Direction MoveIntoDirection { get ; set ; } = Direction.None ;

        /// <inheritdoc />
        public uint StoppingHeight { get ; set ; }

        /// <inheritdoc />
        public uint TargetHeight { get ; set ; }

        /// <inheritdoc />
        public uint Delta { get ; private set ; }

        /// <inheritdoc />
        public Direction StartMovingIntoDirection { get ; set ; }

        /// <inheritdoc />
        public bool HasReachedTargetHeight { get ; private set ; }

        /// <inheritdoc />
        public IHasReachedTargetHeightCalculator Calculate ( )
        {
            Delta = TargetHeight >= StoppingHeight
                        ? ( uint ) Math.Abs ( TargetHeight   - StoppingHeight )
                        : ( uint ) Math.Abs ( StoppingHeight - TargetHeight ) ;

            if ( StartMovingIntoDirection != MoveIntoDirection )
            {
                // must be StoppingHeight must be 'behind' TargetHeight
                HasReachedTargetHeight = true ;

                return this ;
            }

            if ( CalculatePastTargetHeight ( ) )
            {
                HasReachedTargetHeight = true ;

                return this ;
            }

            var isCloseToTargetHeight = Delta <= Math.Abs ( MovementUntilStop ) ;

            HasReachedTargetHeight = MoveIntoDirection switch
                                     {
                                         Direction.Up   => isCloseToTargetHeight || StoppingHeight >= TargetHeight ,
                                         Direction.Down => isCloseToTargetHeight || StoppingHeight <= TargetHeight ,
                                         _              => true
                                     } ;

            _logger.Debug ( ToString ( ) ) ;

            return this ;
        }

        private bool CalculatePastTargetHeight ( )
        {
            switch ( MoveIntoDirection )
            {
                case Direction.Up :
                    if ( StoppingHeight >= TargetHeight )
                        return true ;
                    break ;
                case Direction.Down :
                    if ( StoppingHeight <= TargetHeight )
                        return true ;
                    break ;
                case Direction.None :
                    return true ;
                default :
                    throw new ArgumentOutOfRangeException ( ) ;
            }

            return false ;
        }

        public override string ToString ( )
        {
            return $"{JsonSerializer.Serialize ( this )}" ;
        }

        private readonly ILogger _logger ;
    }
}