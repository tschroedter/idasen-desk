using System.Collections.Generic ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    public class GattServicesDictionary
        : IGattServicesDictionary
    {
        public IGattCharacteristicsResultWrapper this [ IGattDeviceServiceWrapper service ]
        {
            get => _dictionary [ service ] ;
            set => _dictionary [ service ] = value ; // todo can be null, add check?
        }

        public void Clear ( )
        {
            DisposeServices ( ) ;

            _dictionary.Clear ( ) ;
        }

        public void Dispose ( )
        {
            DisposeServices ( ) ;
        }

        public IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper >
            ReadOnlyDictionary =>
            _dictionary ; // todo might not be thread safe

        public int Count => _dictionary.Count ;

        private void DisposeServices ( )
        {
            foreach ( var service in _dictionary.Keys )
            {
                service.Dispose ( ) ;
            }
        }

        private readonly Dictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > _dictionary =
            new Dictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > ( ) ;
    }
}