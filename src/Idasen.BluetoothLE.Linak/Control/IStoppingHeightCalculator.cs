namespace Idasen.BluetoothLE.Linak.Control
{
    public interface IStoppingHeightCalculator
    {
        /// <inheritdoc />
        uint MaxSpeedToStopMovement { get; set; }

        /// <inheritdoc />
        int MaxSpeed { get; set; }

        /// <inheritdoc />
        int Speed { get; set; }

        /// <inheritdoc />
        float FudgeFactor { get; set; }

        /// <inheritdoc />
        uint TargetHeight { get; set; }

        /// <inheritdoc />
        uint Height { get; set; }

        /// <inheritdoc />
        uint Delta { get; }

        /// <inheritdoc />
        uint StoppingHeight { get; }

        /// <inheritdoc />
        int MovementUntilStop { get; }

        /// <inheritdoc />
        bool HasReachedTargetHeight { get; }

        /// <inheritdoc />
        Direction MoveIntoDirection { get; set; }

        Direction StartMovingIntoDirection { get; set; }

        /// <inheritdoc />
        IStoppingHeightCalculator Calculate();
    }
}