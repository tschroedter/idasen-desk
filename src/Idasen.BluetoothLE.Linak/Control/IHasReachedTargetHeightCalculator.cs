namespace Idasen.BluetoothLE.Linak.Control
{
    public interface IHasReachedTargetHeightCalculator
    {
        /// todo
        int MovementUntilStop { get ; set ; }

        /// todo
        Direction MoveIntoDirection { get ; set ; }

        /// todo
        uint StoppingHeight { get ; set ; }

        /// todo
        uint TargetHeight { get ; set ; }

        /// todo
        bool HasReachedTargetHeight { get ; }

        /// <inheritdoc />
        uint Delta { get ; }

        Direction StartMovingIntoDirection { get ; set ; }

        /// todo
        IHasReachedTargetHeightCalculator Calculate ( ) ;
    }
}