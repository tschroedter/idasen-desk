using System;
using System.Threading.Tasks;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Linak.Interfaces;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using JetBrains.Annotations;

namespace Idasen.BluetoothLE.Linak
{
    public class DeskFactory
        : IDeskFactory
    {
        private readonly Func<IDevice, IDesk> _deskFactory;
        private readonly IDeviceFactory       _deviceFactory;

        public DeskFactory(
            [NotNull] IDeviceFactory       deviceFactory,
            [NotNull] Func<IDevice, IDesk> deskFactory)
        {
            Guard.ArgumentNotNull(deviceFactory,
                                  nameof(deviceFactory));
            Guard.ArgumentNotNull(deskFactory,
                                  nameof(deskFactory));

            _deviceFactory = deviceFactory;
            _deskFactory   = deskFactory;
        }

        public async Task<IDesk> CreateAsync(ulong address) // todo check if other method need to end with Async
        {
            var device = await _deviceFactory.FromBluetoothAddressAsync(address);

            return _deskFactory.Invoke(device);
        }
    }
}