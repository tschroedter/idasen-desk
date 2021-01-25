using System ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Control
{
    /// <inheritdoc />
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class StoppingHeightCalculator
        : IStoppingHeightCalculator
    {
        public StoppingHeightCalculator ( [ NotNull ] ILogger                           logger ,
                                          [ NotNull ] IHasReachedTargetHeightCalculator calculator )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( calculator ,
                                    nameof ( calculator ) ) ;

            _logger     = logger ;
            _calculator = calculator ;
        }

        /// <inheritdoc />
        public uint MaxSpeedToStopMovement { get ; set ; } = DefaultMaxSpeedToStopMovement ;

        /// <inheritdoc />
        public int MaxSpeed { get ; set ; } = DefaultMaxSpeed ;

        /// <inheritdoc />
        public int Speed { get ; set ; }

        /// <inheritdoc />
        public float FudgeFactor { get ; set ; } = 2.0f ;

        /// <inheritdoc />
        public uint TargetHeight { get ; set ; }

        /// <inheritdoc />
        public uint Height { get ; set ; }

        /// <inheritdoc />
        public uint Delta { get ; private set ; }

        /// <inheritdoc />
        public uint StoppingHeight { get ; private set ; }

        /// <inheritdoc />
        public int MovementUntilStop { get ; set ; }

        /// <inheritdoc />
        public bool HasReachedTargetHeight { get ; private set ; }

        /// <inheritdoc />
        public Direction MoveIntoDirection { get ; set ; } = Direction.None ;

        /// <inheritdoc />
        public Direction StartMovingIntoDirection { get ; set ; }

        /// <inheritdoc />
        public IStoppingHeightCalculator Calculate ( )
        {
            MoveIntoDirection = CalculateMoveIntoDirection ( ) ;

            _logger.Information ( $"Height = {Height},"                                     +
                                  $"Speed = {Speed},"                                       +
                                  $"StartMovingIntoDirection = {StartMovingIntoDirection} " +
                                  $"MoveIntoDirection = {MoveIntoDirection}" ) ;

            if ( Speed == 0 )
                CalculateForSpeedZero ( ) ;
            else
                CalculateForSpeed ( ) ;

            return this ;
        }

        private const int DefaultMaxSpeedToStopMovement = 14 ;   // per notification, 16 notifications in 60 secs
        private const int DefaultMaxSpeed               = 6200 ; // rpm/10

        private Direction CalculateMoveIntoDirection ( )
        {
            if ( Math.Abs ( ( int ) Height - ( int ) TargetHeight ) <= MaxSpeedToStopMovement * FudgeFactor )
                return Direction.None ;

            return Height > TargetHeight
                       ? Direction.Down
                       : Direction.Up ;
        }

        private void CalculateForSpeed ( )
        {
            MovementUntilStop = DefaultMaxSpeedToStopMovement ;

            StoppingHeight = Height ;

            MovementUntilStop = ( int ) ( ( float ) Speed / MaxSpeed *
                                          MaxSpeedToStopMovement     * FudgeFactor ) ;

            StoppingHeight = ( uint ) ( StoppingHeight + MovementUntilStop ) ;

            var (hasReachedTargetHeight , delta) = CalculateHasReachedTargetHeight ( ) ;

            Delta                  = delta ;
            HasReachedTargetHeight = hasReachedTargetHeight ;

            LogStatus ( ) ;
        }

        private void LogStatus ( )
        {
            _logger.Information ( $"Height = {Height}, "                      +
                                  $"Speed = {Speed} "                         +
                                  $"TargetHeight = {TargetHeight}, "          +
                                  $"StoppingHeight = {StoppingHeight} "       +
                                  $"MovementUntilStop = {MovementUntilStop} " +
                                  $"Delta = {Delta:F2}" ) ;
        }

        private void CalculateForSpeedZero ( )
        {
            MovementUntilStop = 0 ;

            StoppingHeight = Height ;

            var (hasReachedTargetHeight , delta) = CalculateHasReachedTargetHeight ( ) ;

            Delta                  = delta ;
            HasReachedTargetHeight = hasReachedTargetHeight ;

            LogStatus ( ) ;
        }

        private (bool hasReachedTargetHeight , uint delta) CalculateHasReachedTargetHeight ( )
        {
            _calculator.TargetHeight             = TargetHeight ;
            _calculator.StoppingHeight           = StoppingHeight ;
            _calculator.MovementUntilStop        = MovementUntilStop ;
            _calculator.MoveIntoDirection        = MoveIntoDirection ;
            _calculator.StartMovingIntoDirection = StartMovingIntoDirection ;
            _calculator.Calculate ( ) ;

            return ( _calculator.HasReachedTargetHeight , _calculator.Delta ) ;
        }

        private readonly IHasReachedTargetHeightCalculator _calculator ;

        private readonly ILogger _logger ;
    }
}