using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    public interface IGatReadResultWrapperFactory
    {
        /// <summary>
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        IGattReadResultWrapper Create ( GattReadResult result ) ;
    }
}