using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Control
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeskMoverFactory
        : IDeskMoverFactory
    {
        public DeskMoverFactory ( [ NotNull ] DeskMover.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        public IDeskMover Create ( IDeskCommandExecutor executor ,
                                   IDeskHeightAndSpeed  heightAndSpeed )
        {
            Guard.ArgumentNotNull ( executor ,
                                    nameof ( executor ) ) ;
            Guard.ArgumentNotNull ( heightAndSpeed ,
                                    nameof ( heightAndSpeed ) ) ;

            return _factory ( executor ,
                              heightAndSpeed ) ;
        }

        private readonly DeskMover.Factory _factory ;
    }
}