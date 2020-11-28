using System.Diagnostics.CodeAnalysis ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    [ ExcludeFromCodeCoverage ]
    public class GattWriteResultWrapper
        : IGattWriteResult
    {
        public GattWriteResultWrapper ( [ JetBrains.Annotations.NotNull ] GattWriteResult result )
        {
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            _result = result ;
        }

        public GattCommunicationStatus Status => _result.Status ;

        public byte? ProtocolError => _result.ProtocolError ;

        public delegate IGattWriteResult Factory ( GattWriteResult result ) ;

        public static readonly IGattWriteResult NotSupported = new GattWriteResultNotSupported ( ) ;

        private readonly GattWriteResult _result ;
    }
}