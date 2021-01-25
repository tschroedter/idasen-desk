using System ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DescriptionToUuid
        : SimpleDictionaryBase < string , Guid > ,
          IDescriptionToUuid
    {
    }
}