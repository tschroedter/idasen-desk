namespace Idasen.BluetoothLE.Linak.Control
{
    public interface IStoppingHeightCalculator
    {
        /// <summary>
        ///     The expected movement in mm of the desk from maximum speed to zero
        ///     speed.
        /// </summary>
        uint MaxSpeedToStopMovement { get ; set ; }

        /// <summary>
        ///     The maximum movement in mm speed of the desk.
        /// </summary>
        int MaxSpeed { get ; set ; }

        /// <summary>
        ///     The current movement speed of the desk.
        /// </summary>
        int Speed { get ; set ; }

        /// <summary>
        ///     The Fudge factor used to calculate the stopping height.
        /// </summary>
        float FudgeFactor { get ; set ; }

        /// <summary>
        ///     The target height in mm to reach by the desk.
        /// </summary>
        uint TargetHeight { get ; set ; }

        /// <summary>
        ///     The current height of the desk.
        /// </summary>
        uint Height { get ; set ; }

        /// <summary>
        ///     Delta between the TargetHeight and StoppingHeight
        /// </summary>
        uint Delta { get ; }

        /// <summary>
        ///     The estimated stopping height of the desk in motion.
        /// </summary>
        uint StoppingHeight { get ; }

        /// <summary>
        ///     The estimated movement of the desk until it stops.
        /// </summary>
        int MovementUntilStop { get ; }

        /// <summary>
        ///     Indicates if the current desk height is available or not.
        /// </summary>
        bool HasReachedTargetHeight { get ; }

        /// <summary>
        ///     Move the desk into the given direction.
        /// </summary>
        Direction MoveIntoDirection { get ; set ; }

        /// <summary>
        ///     Direction to start moving into.
        /// </summary>
        Direction StartMovingIntoDirection { get ; set ; }

        /// <summary>
        ///     Calculate the get only properties: Delta, StoppingHeight...
        /// </summary>
        /// <returns>
        ///     Itself.
        /// </returns>
        IStoppingHeightCalculator Calculate ( ) ;
    }
}