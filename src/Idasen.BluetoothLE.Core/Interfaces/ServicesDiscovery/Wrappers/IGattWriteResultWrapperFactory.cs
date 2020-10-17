using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    public interface IGattWriteResultWrapperFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        IGattWriteResult Create ( GattWriteResult result ) ;
    }
}