using System.Threading.Tasks;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;

namespace Idasen.BluetoothLE.Core.Interfaces
{
    public interface IMatchMaker
    {
        /// <summary>
        ///     Attempts to pair to BLE device by address.
        /// </summary>
        /// <param name="address">The BLE device address.</param>
        /// <returns></returns>
        Task<IDevice> PairToDeviceAsync(ulong address);
    }
}