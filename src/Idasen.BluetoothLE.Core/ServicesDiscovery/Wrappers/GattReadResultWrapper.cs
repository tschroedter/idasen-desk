using System.Diagnostics.CodeAnalysis ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    [ ExcludeFromCodeCoverage ]
    public class GattReadResultWrapper
        : IGattReadResultWrapper
    {
        public GattReadResultWrapper ( [ JetBrains.Annotations.NotNull ] GattReadResult result )
        {
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            _result = result ;
        }

        public GattCommunicationStatus Status => _result.Status ;

        public byte?   ProtocolError => _result.ProtocolError ;
        public IBuffer Value         => _result.Value ;

        public delegate IGattReadResultWrapper Factory ( GattReadResult result ) ;

        public static readonly IGattReadResultWrapper NotSupported = new GattReadResultWrapperNotSupported ( ) ;

        private readonly GattReadResult _result ;
    }
}