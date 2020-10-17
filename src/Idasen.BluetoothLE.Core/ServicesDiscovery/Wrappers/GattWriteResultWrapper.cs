using System.Diagnostics.CodeAnalysis ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    [ ExcludeFromCodeCoverage ]
    public class GattWriteResultWrapper
        : IGattWriteResult
    {
        public static readonly IGattWriteResult NotSupported = new GattWriteResultNotSupported (  );

        public GattWriteResultWrapper ( [ NotNull ] GattWriteResult result )
        {
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            _result = result ;
        }

        public GattCommunicationStatus Status => _result.Status ;

        public byte? ProtocolError => _result.ProtocolError ;

        public delegate IGattWriteResult Factory ( GattWriteResult result ) ;

        private readonly GattWriteResult _result ;
    }
}