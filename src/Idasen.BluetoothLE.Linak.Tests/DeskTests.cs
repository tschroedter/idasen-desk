using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class DeskTests
    {
        [ AutoDataTestMethod ]
        public void Connect_ForInvoked_CallsDeskConnector (
            Desk                      sut ,
            [ Freeze ] IDeskConnector connector )
        {
            sut.Connect ( ) ;

            connector.Received ( )
                     .Connect ( ) ;
        }

        [ AutoDataTestMethod ]
        public void MoveTo_ForTargetHeight_CallsConnector (
            Desk                      sut ,
            [ Freeze ] IDeskConnector connector )
        {
            const uint targetHeight = 100u ;

            sut.MoveTo ( targetHeight ) ;

            connector.Received ( )
                     .MoveTo ( targetHeight ) ;
        }

        [ AutoDataTestMethod ]
        public void MoveUp_ForInvoked_CallsConnector (
            Desk                      sut ,
            [ Freeze ] IDeskConnector connector )
        {
            sut.MoveUp ( ) ;

            connector.Received ( )
                     .MoveUp ( ) ;
        }

        [ AutoDataTestMethod ]
        public void MoveDown_ForInvoked_CallsConnector (
            Desk                      sut ,
            [ Freeze ] IDeskConnector connector )
        {
            sut.MoveDown ( ) ;

            connector.Received ( )
                     .MoveDown ( ) ;
        }

        [ AutoDataTestMethod ]
        public void MoveStop_ForInvoked_CallsConnector (
            Desk                      sut ,
            [ Freeze ] IDeskConnector connector )
        {
            sut.MoveStop ( ) ;

            connector.Received ( )
                     .MoveStop ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Dispose_ForInvoked_CallsDeskConnector (
            Desk                      sut ,
            [ Freeze ] IDeskConnector connector )
        {
            sut.Dispose ( ) ;

            connector.Received ( )
                     .Dispose ( ) ;
        }

        [ AutoDataTestMethod ]
        public void DeviceName_ForInvoked_DeviceName (
            Desk              sut ,
            [ Freeze ] string deviceName )
        {
            sut.DeviceName
               .Should ( )
               .Be ( deviceName ) ;
        }

        [ AutoDataTestMethod ]
        public void BluetoothAddress_ForInvoked_BluetoothAddress (
            Desk             sut ,
            [ Freeze ] ulong bluetoothAddress )
        {
            sut.BluetoothAddress
               .Should ( )
               .Be ( bluetoothAddress ) ;
        }

        [ AutoDataTestMethod ]
        public void BluetoothAddressType_ForInvoked_BluetoothAddressType (
            Desk              sut ,
            [ Freeze ] string bluetoothAddressType )
        {
            sut.BluetoothAddressType
               .Should ( )
               .Be ( bluetoothAddressType ) ;
        }
    }
}