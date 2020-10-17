using System;

namespace Idasen.BluetoothLE.Linak.Control
{
    public class HasReachedTargetHeightCalculator
        : IHasReachedTargetHeightCalculator
    {
        /// <inheritdoc />
        public int MovementUntilStop { get; set; }

        /// <inheritdoc />
        public Direction MoveIntoDirection { get; set; } = Direction.None;

        /// <inheritdoc />
        public uint StoppingHeight { get; set; }

        /// <inheritdoc />
        public uint TargetHeight { get; set; }

        /// <inheritdoc />
        public uint Delta { get; private set; }

        /// <inheritdoc />
        public Direction StartMovingIntoDirection { get; set; }

        /// <inheritdoc />
        public bool HasReachedTargetHeight { get; private set; }

        public IHasReachedTargetHeightCalculator Calculate()
        {
            Delta = TargetHeight >= StoppingHeight
                        ? (uint)Math.Abs(TargetHeight   - StoppingHeight)
                        : (uint)Math.Abs(StoppingHeight - TargetHeight);

            if (StartMovingIntoDirection != MoveIntoDirection)
            {
                // must be StoppingHeight must be 'behind' TargetHeight
                HasReachedTargetHeight = true;

                return this;
            }

            if (CalculatePastTargetHeight())
            {
                HasReachedTargetHeight = true;

                return this;
            }

            var isCloseToTargetHeight = Delta <= Math.Abs(MovementUntilStop);

            HasReachedTargetHeight = MoveIntoDirection switch
                                     {
                                         Direction.Up   => isCloseToTargetHeight || StoppingHeight >= TargetHeight,
                                         Direction.Down => isCloseToTargetHeight || StoppingHeight <= TargetHeight,
                                         _              => true
                                     };

            return this;
        }

        private bool CalculatePastTargetHeight()
        {
            switch (MoveIntoDirection)
            {
                case Direction.Up:
                    if (StoppingHeight >= TargetHeight)
                        return true;
                    break;
                case Direction.Down:
                    if (StoppingHeight <= TargetHeight)
                        return true;
                    break;
                case Direction.None:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }
    }
}