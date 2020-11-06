using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using Idasen.BluetoothLE.Linak.Control;
using Idasen.BluetoothLE.Linak.Interfaces;
using JetBrains.Annotations;
using Serilog;

namespace Idasen.BluetoothLE.Linak
{
    public class Desk
        : IDesk
    {
        public Desk(
            [NotNull] ILogger                           logger,
            [NotNull] IScheduler                        scheduler,
            [NotNull] Func<ISubject<IEnumerable<byte>>> subjectFactory,
            [NotNull] ISubject<uint>                    subjectHeight,
            [NotNull] ISubject<int>                     subjectSpeed,
            [NotNull] ISubject<bool>                    subjectRefreshed,
            [NotNull] ISubject<HeightSpeedDetails>      subjectHeightAndSpeed,
            [NotNull] IDevice                           device,
            [NotNull] IDeskCharacteristics              deskCharacteristics,
            [NotNull] IDeskHeightAndSpeedFactory        heightAndSpeedFactory,
            [NotNull] IDeskCommandExecutorFactory       commandExecutorFactory,
            [NotNull] IDeskMoverFactory                 moverFactory)
        {
            Guard.ArgumentNotNull(logger,
                                  nameof(logger));
            Guard.ArgumentNotNull(scheduler,
                                  nameof(scheduler));
            Guard.ArgumentNotNull(subjectFactory,
                                  nameof(subjectFactory));
            Guard.ArgumentNotNull(subjectHeight,
                                  nameof(subjectHeight));
            Guard.ArgumentNotNull(subjectRefreshed,
                                  nameof(subjectRefreshed));
            Guard.ArgumentNotNull(subjectSpeed,
                                  nameof(subjectSpeed));
            Guard.ArgumentNotNull(subjectHeightAndSpeed,
                                  nameof(subjectHeightAndSpeed));
            Guard.ArgumentNotNull(device,
                                  nameof(device));
            Guard.ArgumentNotNull(deskCharacteristics,
                                  nameof(deskCharacteristics));
            Guard.ArgumentNotNull(heightAndSpeedFactory,
                                  nameof(heightAndSpeedFactory));
            Guard.ArgumentNotNull(commandExecutorFactory,
                                  nameof(commandExecutorFactory));
            Guard.ArgumentNotNull(moverFactory,
                                  nameof(moverFactory));

            _logger                 = logger;
            _scheduler              = scheduler;
            _subjectHeight          = subjectHeight;
            _subjectSpeed           = subjectSpeed;
            _subjectRefreshed       = subjectRefreshed;
            _subjectHeightAndSpeed  = subjectHeightAndSpeed;
            _device                 = device;
            _deskCharacteristics    = deskCharacteristics;
            _heightAndSpeedFactory  = heightAndSpeedFactory;
            _commandExecutorFactory = commandExecutorFactory;
            _moverFactory           = moverFactory;

            _device.GattServicesRefreshed
                   .SubscribeOn(scheduler)
                   .Subscribe(OnGattServicesRefreshed);

            DeviceNameChanged = subjectFactory();
        }

        public void Connect()
        {
            _device.Connect();
        }

        /// <inheritdoc />
        public ISubject<IEnumerable<byte>> DeviceNameChanged { get; }

        /// <inheritdoc />
        public IObservable<uint> HeightChanged => _subjectHeight;

        /// <inheritdoc />
        public IObservable<int> SpeedChanged => _subjectSpeed;

        /// <inheritdoc />
        public IObservable<HeightSpeedDetails> HeightAndSpeedChanged => _subjectHeightAndSpeed; // todo use only this

        /// <inheritdoc />
        public IObservable<uint> Finished => _mover.Finished;

        /// <inheritdoc />
        public IObservable<bool> RefreshedChanged => _subjectRefreshed;

        public void MoveTo(uint targetHeight)
        {
            if (_mover == null)
            {
                _logger.Error("Desk needs to be refreshed first!");
                return; // todo logging
            }

            _mover.TargetHeight = targetHeight;
            _mover.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposableHeightAndSpeed?.Dispose();
            _disposableSpeed?.Dispose();
            _disposableHeight?.Dispose();
            _heightAndSpeed?.Dispose();
            _subscriber?.Dispose();
        }

        private async void OnGattServicesRefreshed(GattCommunicationStatus status)
        {
            _logger.Information($"[{_device.DeviceId}] "                         +
                                $"ConnectionStatus: {_device.ConnectionStatus} " +
                                $"GattCommunicationStatus: {_device.GattCommunicationStatus}");

            _deskCharacteristics.Initialize(_device);

            _subscriber?.Dispose();

            _subscriber = _deskCharacteristics.GenericAccess
                                              .DeviceNameChanged
                                              .SubscribeOn(_scheduler)
                                              .Subscribe(OnDeviceNameChanged);

            await _deskCharacteristics.Refresh();

            _heightAndSpeed?.Dispose();
            _heightAndSpeed = _heightAndSpeedFactory.Create(_deskCharacteristics.ReferenceOutput);
            _heightAndSpeed.Initialize ( ) ;

            _disposableHeight = _heightAndSpeed.HeightChanged
                                               .SubscribeOn(_scheduler)
                                               .Subscribe(height => _subjectHeight.OnNext(height));
            _disposableSpeed = _heightAndSpeed.SpeedChanged
                                              .SubscribeOn(_scheduler)
                                              .Subscribe(speed => _subjectSpeed.OnNext(speed));
            _disposableHeightAndSpeed = _heightAndSpeed.HeightAndSpeedChanged
                                                       .SubscribeOn(_scheduler)
                                                       .Subscribe(details => _subjectHeightAndSpeed.OnNext(details));

            _executer = _commandExecutorFactory.Create(_deskCharacteristics.Control);
            _mover = _moverFactory.Create(_executer,
                                          _heightAndSpeed);
            _mover.Initialize (  );

            _subjectRefreshed.OnNext(true);
        }

        private void OnDeviceNameChanged(IEnumerable<byte> value)
        {
            DeviceNameChanged.OnNext(value);
        }

        private readonly IDeskCommandExecutorFactory  _commandExecutorFactory;
        private readonly IDeskCharacteristics         _deskCharacteristics;
        private readonly IDevice                      _device;
        private readonly IDeskHeightAndSpeedFactory   _heightAndSpeedFactory;
        private readonly ILogger                      _logger;
        private readonly IDeskMoverFactory            _moverFactory;
        private readonly IScheduler                   _scheduler;
        private readonly ISubject<uint>               _subjectHeight;
        private readonly ISubject<HeightSpeedDetails> _subjectHeightAndSpeed;
        private readonly ISubject<bool>               _subjectRefreshed;
        private readonly ISubject<int>                _subjectSpeed;
        private          IDisposable                  _disposableHeight;
        private          IDisposable                  _disposableHeightAndSpeed;
        private          IDisposable                  _disposableSpeed;
        private          IDeskCommandExecutor         _executer;

        private IDeskHeightAndSpeed _heightAndSpeed;
        private IDeskMover          _mover;
        private IDisposable         _subscriber;
    }
}