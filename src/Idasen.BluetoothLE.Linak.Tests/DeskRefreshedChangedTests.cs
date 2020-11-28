using System ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.Reactive.Testing ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class DeskRefreshedChangedTests
        : DeskRaiseEventForDeskBase < bool >
    {
        protected override void SetSubscription ( IDesk         desk ,
                                                  TestScheduler scheduler )
        {
            desk.RefreshedChanged
                .ObserveOn ( scheduler )
                .Subscribe ( OnRaised ) ;
        }

        protected override void SetSubject ( IDeskConnector   connector ,
                                             Subject < bool > subject )
        {
            connector.RefreshedChanged
                     .Returns ( subject ) ;
        }
    }
}