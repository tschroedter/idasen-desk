using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Autofac;
using AutofacSerilogIntegration;
using Idasen.BluetoothLE.Characteristics.Common;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery;
using Idasen.BluetoothLE.Linak;
using Idasen.BluetoothLE.Linak.Interfaces;
using Serilog;
using Serilog.Events;

namespace Idasen.ConsoleApp
{
    public class Demo
        : IDisposable
    {
        private readonly IContainer               _container;
        private readonly IDeskFactory             _deskFactory;
        private readonly IDisposable              _deskFound;
        private readonly IDisposable              _discovered;
        private readonly ILogger                  _logger;
        private readonly IDeviceMonitorWithExpiry _monitor;
        private readonly IDisposable              _nameChanged;
        private readonly IDisposable              _updated;

        private IDesk       _desk;
        private IDisposable _subscriberDeskDeviceNameChanged;
        private IDisposable _refreshedChanged;

        public Demo()
        {
            const string template =
                "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message}{NewLine}{Exception}";
            // "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message} (at {Caller}){NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                        .Enrich.WithCaller()
                        .MinimumLevel.Information()
                        .WriteTo
                        .ColoredConsole(LogEventLevel.Debug, template)
                        .CreateLogger();

            var builder = new ContainerBuilder();

            builder.RegisterLogger();
            builder.RegisterModule<BluetoothLECoreModule>();
            builder.RegisterModule<BluetoothLELinakModule>();

            _container = builder.Build();

            _logger      = _container.Resolve<ILogger>();
            _deskFactory = _container.Resolve<IDeskFactory>();
            _monitor     = _container.Resolve<IDeviceMonitorWithExpiry>();

            _updated     = _monitor.DeviceUpdated.Subscribe(OnDeviceUpdated);
            _discovered  = _monitor.DeviceDiscovered.Subscribe(OnDeviceDiscovered);
            _nameChanged = _monitor.DeviceNameUpdated.Subscribe(OnDeviceNameChanged);
            _deskFound = _monitor.DeviceNameUpdated
                                 .Where(device => device.Name.StartsWith("Desk") ||
                                                  device.Address == 250635178951455)
                                 .Subscribe(OnDeskDiscovered);
        }

        public void Dispose()
        {
            _refreshedChanged?.Dispose();
            _subscriberDeskDeviceNameChanged?.Dispose();
            _deskFound?.Dispose(); // todo list
            _nameChanged?.Dispose();
            _discovered?.Dispose();
            _updated?.Dispose();
            _monitor?.Dispose();
            _container?.Dispose();
        }

        public void Start()
        {
            _monitor.Start();
        }

        public void Stop()
        {
            _monitor.Stop();
        }

        private async void OnDeskDiscovered(IDevice device)
        {
            if (_desk != null)
                return;

            try
            {
                _desk = await _deskFactory.CreateAsync(device.Address);
                _subscriberDeskDeviceNameChanged =
                    _desk.DeviceNameChanged.Subscribe(OnDeskDeviceNameChanged);
                _desk.Connect();

                _refreshedChanged = _desk.RefreshedChanged.Subscribe(OnRefreshedChanged); // todo rename On...
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void OnDeskDeviceNameChanged(IEnumerable<byte> value)
        {
            var array = value.ToArray();

            var text = Encoding.UTF8.GetString(array);

            _logger.Information($"Received: {array.ToHex()} - '{text}'");
        }

        private void OnDeviceUpdated(IDevice device)
        {
            _logger.Information($"Device Updated: {device}");
            //Logger.Information($"{DevicesToString(_monitor.DiscoveredDevices)}");
        }

        private void OnDeviceDiscovered(IDevice device)
        {
            _logger.Information($"Device Discovered: {device}");
        }

        private void OnDeviceNameChanged(IDevice device)
        {
            _logger.Information($"Device Name Changed: {device}");
        }

        private void OnRefreshedChanged(bool status)
        {
            _desk.MoveTo(7200u);
        }
    }
}