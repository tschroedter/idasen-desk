using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDesk
        : IDisposable
    {
        /// <summary>
        ///     Connect to a desk. todo TryConnect or event
        /// </summary>
        void Connect();

        /// <summary>
        ///     Raised when the DeviceName changes.
        /// </summary>
        ISubject<IEnumerable<byte>> DeviceNameChanged { get; }

        /// <summary>
        ///     Notifies when the desk's height has changed.
        /// </summary>
        IObservable<uint> HeightChanged { get; }

        /// <summary>
        ///     Notifies when the desk's speed has changed.
        /// </summary>
        IObservable<int> SpeedChanged { get; }

        /// <inheritdoc />
        IObservable<bool> RefreshedChanged { get; }

        /// <summary>
        ///     todo
        /// </summary>
        IObservable<HeightSpeedDetails> HeightAndSpeedChanged { get; }

        /// <summary>
        ///     todo
        /// </summary>
        IObservable<uint> Finished { get; }

        void MoveTo(uint targetHeight);
    }
}