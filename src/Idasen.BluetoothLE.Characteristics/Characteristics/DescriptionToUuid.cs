using System ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public class DescriptionToUuid
        : SimpleDictionaryBase < string , Guid > ,
          IDescriptionToUuid
    {
    }
}