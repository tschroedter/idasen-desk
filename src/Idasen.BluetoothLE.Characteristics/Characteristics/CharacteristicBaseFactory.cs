using Autofac ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public class CharacteristicBaseFactory
        : ICharacteristicBaseFactory
    {
        private readonly ILifetimeScope _scope ;

        public CharacteristicBaseFactory ( [ NotNull ] ILifetimeScope scope )
        {
            Guard.ArgumentNotNull ( scope ,
                                    nameof ( scope ) ) ;

            _scope = scope ;
        }

        public T Create < T > ( IDevice device )
        {
            var instance = _scope.Resolve < T > ( new NamedParameter ( "device" ,
                                                                       device ) ) ;

            return instance ;
        }
    }
}