using System ;
using System.Collections.Generic ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.Reactive.Testing ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class DeskDeviceNameChangedTests
        : DeskRaiseEventForDeskBase < IEnumerable < byte > >
    {
        protected override void SetSubscription ( IDesk         desk ,
                                                  TestScheduler scheduler )
        {
            desk.DeviceNameChanged
                .ObserveOn ( scheduler )
                .Subscribe ( OnRaised ) ;
        }

        protected override void SetSubject ( IDeskConnector                   connector ,
                                             Subject < IEnumerable < byte > > subject )
        {
            connector.DeviceNameChanged
                     .Returns ( subject ) ;
        }
    }
}