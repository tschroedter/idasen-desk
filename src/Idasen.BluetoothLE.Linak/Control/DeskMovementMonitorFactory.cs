using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Control
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeskMovementMonitorFactory
        : IDeskMovementMonitorFactory
    {
        public DeskMovementMonitorFactory ( [ NotNull ] DeskMovementMonitor.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        public IDeskMovementMonitor Create ( IDeskHeightAndSpeed heightAndSpeed )
        {
            Guard.ArgumentNotNull ( heightAndSpeed ,
                                    nameof ( heightAndSpeed ) ) ;

            return _factory ( heightAndSpeed ) ;
        }

        private readonly DeskMovementMonitor.Factory _factory ;
    }
}