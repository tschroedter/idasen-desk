using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Factories
{
    public class ReferenceOutputFactory // todo generic factory
        : IReferenceOutputFactory
    {
        public ReferenceOutputFactory ( [ NotNull ] ReferenceOutput.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        public IReferenceOutput Create ( [ NotNull ] IDevice device )
        {
            Guard.ArgumentNotNull ( device ,
                                    nameof ( device ) ) ;

            return _factory ( device ) ;
        }

        private readonly ReferenceOutput.Factory _factory ;
    }
}