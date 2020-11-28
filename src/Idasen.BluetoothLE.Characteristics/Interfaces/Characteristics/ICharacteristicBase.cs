using System.Threading.Tasks ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface ICharacteristicBase
    {
        T Initialize < T > ( )
            where T : class ;

        public Task Refresh ( ) ;
    }
}