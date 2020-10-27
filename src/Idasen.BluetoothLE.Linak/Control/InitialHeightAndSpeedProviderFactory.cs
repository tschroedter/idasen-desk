using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Control
{
    public class InitialHeightAndSpeedProviderFactory
        : IInitialHeightAndSpeedProviderFactory
    {
        public InitialHeightAndSpeedProviderFactory ( [ NotNull ] InitialHeightProvider.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        public IInitialHeightProvider Create (
            IDeskCommandExecutor executor ,
            IDeskHeightAndSpeed  heightAndSpeed )
        {
            Guard.ArgumentNotNull ( executor ,
                                    nameof ( executor ) ) ;
            Guard.ArgumentNotNull ( heightAndSpeed ,
                                    nameof ( heightAndSpeed ) ) ;

            return _factory ( executor ,
                              heightAndSpeed ) ;
        }

        private readonly InitialHeightProvider.Factory _factory ;
    }
}