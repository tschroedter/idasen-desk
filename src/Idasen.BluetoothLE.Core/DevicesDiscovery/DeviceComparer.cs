using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;

namespace Idasen.BluetoothLE.Core.DevicesDiscovery
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeviceComparer
        : IDeviceComparer
    {
        /// <inheritdoc />
        public bool Equals ( IDevice deviceA ,
                             IDevice deviceB )
        {
            if ( deviceA == null ||
                 deviceB == null )
                return false ;

            return deviceA.BroadcastTime          == deviceB.BroadcastTime &&
                   deviceA.Address                == deviceB.Address       &&
                   deviceA.Name                   == deviceB.Name          &&
                   deviceA.RawSignalStrengthInDBm == deviceB.RawSignalStrengthInDBm ;
        }

        /// <inheritdoc />
        public bool IsEquivalentTo ( IDevice deviceA ,
                                     IDevice deviceB )
        {
            if ( deviceA == null ||
                 deviceB == null )
                return false ;

            return deviceA.Address == deviceB.Address ;
        }
    }
}