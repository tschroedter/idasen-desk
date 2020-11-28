using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;

namespace Idasen.BluetoothLE.Core
{
    /// <inheritdoc />
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