using System.Collections.Generic ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class GattServicesDictionary
        : IGattServicesDictionary
    {
        public IGattCharacteristicsResultWrapper this [ IGattDeviceServiceWrapper service ]
        {
            get
            {
                lock ( _padlock )
                {
                    return _dictionary [ service ] ;
                }
            }
            set
            {
                Guard.ArgumentNotNull ( value ,
                                        nameof ( value ) ) ;

                lock ( _padlock )
                {
                    _dictionary [ service ] = value ;
                }
            }
        }

        public void Clear ( )
        {
            DisposeServices ( ) ;

            lock ( _padlock )
            {
                _dictionary.Clear ( ) ;
            }
        }

        public void Dispose ( )
        {
            DisposeServices ( ) ;
        }

        public IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper >
            ReadOnlyDictionary
        {
            get
            {
                lock ( _padlock )
                {
                    return _dictionary ;
                }
            }
        }

        public int Count
        {
            get
            {
                lock ( _padlock )
                {
                    return _dictionary.Count ;
                }
            }
        }

        private void DisposeServices ( )
        {
            lock ( _padlock )
            {
                foreach ( var service in _dictionary.Keys )
                {
                    service.Dispose ( ) ;
                }
            }
        }

        private readonly Dictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > _dictionary =
            new Dictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > ( ) ;

        private readonly object _padlock = new object ( ) ;
    }
}