using System ;
using System.Collections.Generic ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery
{
    public interface IGattServicesDictionary
        : IDisposable
    {
        IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > ReadOnlyDictionary
        {
            get ;
        }

        IGattCharacteristicsResultWrapper this [ IGattDeviceServiceWrapper service ] { get ; set ; }

        void Clear ( ) ;
    }
}