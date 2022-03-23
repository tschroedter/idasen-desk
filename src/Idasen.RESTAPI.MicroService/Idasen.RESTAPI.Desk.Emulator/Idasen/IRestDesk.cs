namespace Idasen.RESTAPI.Desk.Emulator.Idasen ;

public interface IDesk
    : IDisposable
{
    /// <summary>
    ///     Notifies when the desk's name changed.
    /// </summary>
    IObservable < IEnumerable < byte > > DeviceNameChanged { get ; }

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
    ///     The name of the desk.
    /// </summary>
    string Name { get ; }

    /// <summary>
    ///     The address of the desk.
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
    void MoveUp ( ) ;

    /// <summary>
    ///     Move the desk down.
    /// </summary>
    void MoveDown ( ) ;

    /// <summary>
    ///     Stop moving the desk.
    /// </summary>
    void MoveStop ( ) ;

    /// <summary>
    ///     Lock the desk, stop manual movement.
    /// </summary>
    void MoveLock ( ) ;

    /// <summary>
    ///     Unlock the desk, allow manual movement.
    /// </summary>
    void MoveUnlock ( ) ;
}