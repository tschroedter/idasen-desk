using System.Diagnostics.CodeAnalysis ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    [ExcludeFromCodeCoverage]
    public class GattReadResultWrapper
        : IGattReadResult
    {
        public static readonly IGattReadResult NotSupported = new GattReadResultNotSupported();

        public GattReadResultWrapper([JetBrains.Annotations.NotNull] GattReadResult result)
        {
            Guard.ArgumentNotNull(result,
                                  nameof(result));

            _result = result;
        }

        public GattCommunicationStatus Status => _result.Status;

        public byte?   ProtocolError => _result.ProtocolError;
        public IBuffer Value         => _result.Value ;

        public delegate IGattReadResult Factory(GattReadResult result);

        private readonly GattReadResult _result;
    }
}