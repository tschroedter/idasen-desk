using Idasen.BluetoothLE.Linak.Control ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IHasReachedTargetHeightCalculator
    {
        /// <summary>
        ///     The estimated movement of the desk until it stops.
        /// </summary>
        int MovementUntilStop { get ; set ; }

        /// <summary>
        ///     Move the desk into the given direction.
        /// </summary>
        Direction MoveIntoDirection { get ; set ; }

        /// <summary>
        ///     The estimated stopping height of the desk in motion.
        /// </summary>
        uint StoppingHeight { get ; set ; }

        /// <summary>
        ///     The target height to reach by the desk.
        /// </summary>
        uint TargetHeight { get ; set ; }

        /// <summary>
        ///     'true' if the desk reached the target height, otherwise 'false'.
        /// </summary>
        bool HasReachedTargetHeight { get ; }

        /// <summary>
        ///     Delta between the TargetHeight and StoppingHeight
        /// </summary>
        uint Delta { get ; }

        /// <summary>
        ///     Direction to start moving into.
        /// </summary>
        Direction StartMovingIntoDirection { get ; set ; }

        /// <summary>
        ///     Calculate the properties: Delta and HasReachedTargetHeight.
        /// </summary>
        /// <returns>
        ///     Itself.
        /// </returns>
        IHasReachedTargetHeightCalculator Calculate ( ) ;
    }
}