using System ;
using System.Reactive.Concurrency ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    public interface IObservableTimerFactory
    {
        /// <summary>
        ///     Create a timer based on an IObservable.
        /// </summary>
        /// <param name="period">
        ///     Period for producing the values in the resulting sequence.If
        ///     this value is equal to TimeSpan.Zero, the timer will recur as
        ///     fast as possible.
        /// </param>
        /// <param name="scheduler">
        ///     Scheduler to run the timer on.
        /// </param>
        /// <returns>
        ///     An observable sequence that produces a value after each period.
        /// </returns>
        IObservable < long > Create ( TimeSpan   period ,
                                      IScheduler scheduler ) ;
    }
}