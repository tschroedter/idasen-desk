using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;

namespace Idasen.BluetoothLE.Core
{
    /// <inheritdoc />
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class ObservableTimerFactory
        : IObservableTimerFactory
    {
        /// <inheritdoc />
        public IObservable < long > Create ( TimeSpan   period ,
                                             IScheduler scheduler )
        {
            return Observable.Interval ( period ,
                                         scheduler ) ;
        }
    }
}