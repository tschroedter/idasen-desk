using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Subjects ;
using FluentAssertions ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Selkie.AutoMocking ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.Wrapper
{
    [ AutoDataTestClass ]
    public class GattCharacteristicValueChangedObservablesTests
    {
        [ TestInitialize ]
        public void Initialize ( )
        {
            _logger    = Substitute.For < ILogger > ( ) ;
            _subject   = new Subject < GattCharacteristicValueChangedDetails > ( ) ;
            _scheduler = Substitute.For < IScheduler > ( ) ;
            _details = new GattCharacteristicValueChangedDetails ( Guid.NewGuid ( ) ,
                                                                   Array.Empty < byte > ( ) ,
                                                                   DateTimeOffset.Now ) ;
        }

        [ AutoDataTestMethod ]
        public void Create_ForInvoked_Instance (
            GattCharacteristicValueChangedObservables sut )
        {
            sut.Should ( )
               .NotBeNull ( ) ;
        }

        [ TestMethod ]
        public void ValueChanged_ForSubscribe_Notifies ( )
        {
            var uuid = Guid.Empty ;

            var sut = new GattCharacteristicValueChangedObservables ( _logger ,
                                                                      _scheduler ,
                                                                      _subject ) ;
            using var disposable = sut.ValueChanged
                                      .Subscribe ( x => { uuid = x.Uuid ; } ) ;

            _subject.OnNext ( _details ) ;

            uuid.Should ( )
                .Be ( _details.Uuid ) ;
        }

        private GattCharacteristicValueChangedDetails              _details ;
        private ILogger                                            _logger ;
        private IScheduler                                         _scheduler ;
        private ISubject < GattCharacteristicValueChangedDetails > _subject ;
    }
}