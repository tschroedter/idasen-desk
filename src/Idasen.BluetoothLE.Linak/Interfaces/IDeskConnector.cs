using System ;
using System.Collections.Generic ;
using System.Threading.Tasks ;

namespace Idasen.BluetoothLE.Linak.Interfaces
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
        IObservable < IEnumerable < byte > > DeviceNameChanged { get ; }

        /// <summary>
        ///     The address of the device.
        /// </summary>
        ulong BluetoothAddress { get ; }

        /// <summary>
        ///     The address type.
        /// </summary>
        string BluetoothAddressType { get ; }


        /// <summary>
        ///     The device name.
        /// </summary>
        string DeviceName { get ; }

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
        void MoveTo ( uint targetHeight ) ;

        /// <summary>
        ///     Move the desk up.
        /// </summary>
        /// <returns>'true' if successful otherwise 'false'.</returns>
        Task< bool > MoveUp ( ) ;

        /// <summary>
        ///     Move the desk down.
        /// </summary>
        /// <returns>'true' if successful otherwise 'false'.</returns>
        Task< bool > MoveDown ( ) ;

        /// <summary>
        ///     Stop moving the desk.
        /// </summary>
        /// <returns>'true' if successful otherwise 'false'.</returns>
        Task< bool > MoveStop ( ) ;

        /// <summary>
        ///     Lock the desk which means manual movement is blocked.
        /// </summary>
        /// <returns>'true' if successful otherwise 'false'.</returns>
        Task < bool > MoveLock ( ) ;

        /// <summary>
        ///     Unlock the desk which means manual movement is allowed.
        /// </summary>
        /// <returns>'true' if successful otherwise 'false'.</returns>
        Task< bool > MoveUnlock ( );
    }
}