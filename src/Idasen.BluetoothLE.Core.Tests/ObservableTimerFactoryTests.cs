using System ;
using System.Reactive.Concurrency ;
using FluentAssertions ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Core.Tests
{
    [ AutoDataTestClass ]
    public class ObservableTimerFactoryTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForInvoked_ReturnsInstance (
            ObservableTimerFactory sut ,
            IScheduler             scheduler )
        {
            sut.Create ( TimeSpan.FromSeconds ( 10 ) ,
                         scheduler )
               .Should ( )
               .NotBeNull ( ) ;
        }
    }
}