using System ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface IDescriptionToUuid
        : ISimpleDictionary < string , Guid >
    {
    }
}