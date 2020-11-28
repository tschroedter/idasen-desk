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
    public class DeskHeightAndSpeedChangedTests
        : DeskRaiseEventForDeskBase < HeightSpeedDetails >
    {
        protected override void SetSubscription ( IDesk         desk ,
                                                  TestScheduler scheduler )
        {
            desk.HeightAndSpeedChanged
                .ObserveOn ( scheduler )
                .Subscribe ( OnRaised ) ;
        }

        protected override void SetSubject ( IDeskConnector                 connector ,
                                             Subject < HeightSpeedDetails > subject )
        {
            connector.HeightAndSpeedChanged
                     .Returns ( subject ) ;
        }
    }
}