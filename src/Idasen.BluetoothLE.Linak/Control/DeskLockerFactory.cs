using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Control
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeskLockerFactory
        : IDeskLockerFactory
    {
        public DeskLockerFactory ( [ NotNull ] DeskLocker.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        public IDeskLocker Create ( IDeskMover           deskMover ,
                                    IDeskCommandExecutor executer ,
                                    IDeskHeightAndSpeed  heightAndSpeed )
        {
            Guard.ArgumentNotNull ( deskMover ,
                                    nameof ( deskMover ) ) ;
            Guard.ArgumentNotNull ( executer ,
                                    nameof ( executer ) ) ;
            Guard.ArgumentNotNull ( heightAndSpeed ,
                                    nameof ( heightAndSpeed ) ) ;

            return _factory ( deskMover ,
                              executer ,
                              heightAndSpeed ) ;
        }

        private readonly DeskLocker.Factory _factory ;
    }
}