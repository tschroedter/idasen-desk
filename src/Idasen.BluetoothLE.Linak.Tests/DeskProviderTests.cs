using System ;
using System.Threading ;
using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class DeskProviderTests
    {
        [ AutoDataTestMethod ]
        public void Initialize_ForDeviceNameIsNull_Throws (
            DeskProvider sut ,
            ulong        deviceAddress ,
            uint         deviceTimeout )
        {
            Action action = ( ) => sut.Initialize ( null! ,
                                                    deviceAddress ,
                                                    deviceTimeout ) ;

            action.Should ( )
                  .Throw < ArgumentException > ( )
                  .WithParameter ( "deviceName" ) ;
        }

        [ AutoDataTestMethod ]
        public void Initialize_ForInvoked_CallsDetectorInitialize (
            DeskProvider             sut ,
            [ Freeze ] IDeskDetector detector ,
            string                   deviceName ,
            ulong                    deviceAddress ,
            uint                     deviceTimeout )
        {
            sut.Initialize ( deviceName ,
                             deviceAddress ,
                             deviceTimeout ) ;

            detector.Received ( )
                    .Initialize ( deviceName ,
                                  deviceAddress ,
                                  deviceTimeout ) ;
        }

        [ AutoDataTestMethod ]
        public void Initialize_ForInvoked_SubscribesToDeskDetected (
            DeskProvider                     sut ,
            [ Freeze ] IObservable < IDesk > deskDetected ,
            string                           deviceName ,
            ulong                            deviceAddress ,
            uint                             deviceTimeout )
        {
            sut.Initialize ( deviceName ,
                             deviceAddress ,
                             deviceTimeout ) ;

            deskDetected.ReceivedWithAnyArgs ( )
                        .Subscribe ( ) ;
        }

        [ AutoDataTestMethod ]
        public void StartDetecting_ForInvoked_CallsDeskDetectorStart (
            DeskProvider             sut ,
            [ Freeze ] IDeskDetector detector ,
            string                   deviceName ,
            ulong                    deviceAddress ,
            uint                     deviceTimeout )
        {
            sut.Initialize ( deviceName ,
                             deviceAddress ,
                             deviceTimeout )
               .StartDetecting ( ) ;

            detector.Received ( )
                    .Start ( ) ;
        }

        [ AutoDataTestMethod ]
        public void StopDetecting_ForInvoked_CallsDeskDetectorStart (
            DeskProvider             sut ,
            [ Freeze ] IDeskDetector detector ,
            string                   deviceName ,
            ulong                    deviceAddress ,
            uint                     deviceTimeout )
        {
            sut.Initialize ( deviceName ,
                             deviceAddress ,
                             deviceTimeout )
               .StopDetecting ( ) ;

            detector.Received ( )
                    .Stop ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Dispose_ForInvoked_DisposesDeskDetected (
            DeskProvider           sut ,
            [ Freeze ] IDisposable deskDetected ,
            string                 deviceName ,
            ulong                  deviceAddress ,
            uint                   deviceTimeout )
        {
            sut.Initialize ( deviceName ,
                             deviceAddress ,
                             deviceTimeout )
               .Dispose ( ) ;

            deskDetected.Received ( )
                        .Dispose ( ) ;
        }

        [ AutoDataTestMethod ]
        public void StopDetecting_ForInvoked_DisposesDeskDetector (
            DeskProvider             sut ,
            [ Freeze ] IDeskDetector detector ,
            string                   deviceName ,
            ulong                    deviceAddress ,
            uint                     deviceTimeout )
        {
            sut.Initialize ( deviceName ,
                             deviceAddress ,
                             deviceTimeout )
               .Dispose ( ) ;

            detector.Received ( )
                    .Dispose ( ) ;
        }

        [ AutoDataTestMethod ]
        public void DeskDetected_ForInvoked_CallsDeskDetector (
            DeskProvider                     sut ,
            [ Freeze ] IObservable < IDesk > deskDetected )
        {
            sut.DeskDetected
               .Subscribe ( ) ;

            deskDetected.ReceivedWithAnyArgs ( )
                        .Subscribe ( ) ;
        }


        [ AutoDataTestMethod ]
        public async Task TryGetDesk_ForInvoked_CallsDeskDetectorStart (
            DeskProvider             sut ,
            [ Freeze ] IDeskDetector detector ,
            CancellationTokenSource  source ,
            string                   deviceName ,
            ulong                    deviceAddress ,
            uint                     deviceTimeout )
        {
            await sut.Initialize ( deviceName ,
                                   deviceAddress ,
                                   deviceTimeout )
                     .TryGetDesk ( source.Token ) ;

            detector.Received ( )
                    .Start ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryGetDesk_ForCancelled_ReturnsFalse (
            DeskProvider             sut ,
            [ Freeze ] IDeskDetector detector ,
            CancellationTokenSource  source ,
            string                   deviceName ,
            ulong                    deviceAddress ,
            uint                     deviceTimeout )
        {
            detector.When ( x => x.Initialize ( deviceName ,
                                                deviceAddress ,
                                                deviceTimeout ) )
                    .Do ( info => { source.Cancel ( ) ; } ) ;

            var (success , _) = await sut.Initialize ( deviceName ,
                                                       deviceAddress ,
                                                       deviceTimeout )
                                         .TryGetDesk ( source.Token ) ;

            success.Should ( )
                   .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryGetDesk_ForCancelled_ReturnsNullForDesk (
            DeskProvider             sut ,
            [ Freeze ] IDeskDetector detector ,
            CancellationTokenSource  source ,
            string                   deviceName ,
            ulong                    deviceAddress ,
            uint                     deviceTimeout )
        {
            detector.When ( x => x.Initialize ( deviceName ,
                                                deviceAddress ,
                                                deviceTimeout ) )
                    .Do ( info => { source.Cancel ( ) ; } ) ;

            var (_ , desk) = await sut.Initialize ( deviceName ,
                                                    deviceAddress ,
                                                    deviceTimeout )
                                      .TryGetDesk ( source.Token ) ;

            desk.Should ( )
                .BeNull ( ) ;
        }

        [ AutoDataTestMethod ]
        public void OnDeskDetected_ForInvoked_CallsDeskDetectorStop (
            DeskProvider             sut ,
            [ Freeze ] IDeskDetector detector ,
            IDesk                    desk )
        {
            sut.OnDeskDetected ( desk ) ;

            detector.Received ( )
                    .Stop ( ) ;
        }

        [ AutoDataTestMethod ]
        public void OnDeskDetected_ForInvoked_SetsDesk (
            DeskProvider sut ,
            IDesk        desk )
        {
            sut.OnDeskDetected ( desk ) ;

            sut.Desk
               .Should ( )
               .Be ( desk ) ;
        }

        [ AutoDataTestMethod ]
        public void OnDeskDetected_ForInvoked_CallsDeskDetectedEventSet (
            DeskProvider            sut ,
            IDesk                   desk ,
            CancellationTokenSource source )
        {
            Task.Run ( ( ) => sut.DoTryGetDesk ( source.Token ) ) ;

            Task.Run ( ( ) => sut.OnDeskDetected ( desk ) ,
                       source.Token ) ;

            Thread.Sleep ( 500 ) ; // not a good way but the only way at the moment

            sut.Desk
               .Should ( )
               .Be ( desk ) ;

            source.Cancel ( false ) ;
        }
    }
}