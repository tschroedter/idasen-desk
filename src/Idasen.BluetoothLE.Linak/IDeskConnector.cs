using System ;
using System.Collections.Generic ;
using System.Reactive.Subjects ;

namespace Idasen.BluetoothLE.Linak
{
    public interface IDeskConnector
        : IDisposable
    {
        /// <summary>
        ///     Notifies when the desk's height changed.
        /// </summary>
        IObservable < uint > HeightChanged { get ; }

        /// <summary>
        ///     Notifies when the desk's speed changed.
        /// </summary>
        IObservable < int > SpeedChanged { get ; }

        /// <summary>
        ///     Notifies when the desk's height and speed changed.
        /// </summary>
        IObservable < HeightSpeedDetails > HeightAndSpeedChanged { get ; }

        /// <summary>
        ///     Notifies when the desk finished moving.
        /// </summary>
        IObservable < uint > FinishedChanged { get ; }

        /// <summary>
        ///     Notifies when the desk's internal components have changed.
        /// </summary>
        IObservable < bool > RefreshedChanged { get ; }

        /// <summary>
        ///     Notifies when the desk's name changed.
        /// </summary>
        ISubject < IEnumerable < byte > > DeviceNameChanged { get ; }

        /// <summary>
        ///     Connect to a desk.
        /// </summary>
        void Connect ( ) ;

        /// <summary>
        ///     Move desk to the given height.
        /// </summary>
        /// <param name="targetHeight">
        ///     The target height.
        /// </param>
        void MoveTo ( in uint targetHeight ) ;

        /// <summary>
        ///     Move the desk up.
        /// </summary>
        void MoveUp ( ) ;

        /// <summary>
        ///     Move the desk down.
        /// </summary>
        void MoveDown ( ) ;

        /// <summary>
        ///     Stop moving the desk.
        /// </summary>
        void MoveStop ( ) ;
    }
}