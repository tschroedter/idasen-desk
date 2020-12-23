using System ;
using System.Threading.Tasks ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class DeskCharacteristicsTests
    {
        [ AutoDataTestMethod ]
        public async Task Refresh_ForInvoked_CallsCharacteristicRefresh (
            DeskCharacteristics sut ,
            IGenericAccess      genericAccess ,
            IGenericAttribute   genericAttribute )
        {
            sut.WithCharacteristics ( DeskCharacteristicKey.GenericAccess ,
                                      genericAccess ) ;
            sut.WithCharacteristics ( DeskCharacteristicKey.GenericAttribute ,
                                      genericAttribute ) ;

            await sut.Refresh ( ) ;

            using var scope = new AssertionScope ( ) ;

            await genericAccess.Received ( )
                               .Refresh ( ) ;

            await genericAttribute.Received ( )
                                  .Refresh ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Initialize_ForInvoked_CallsCreator (
            DeskCharacteristics                    sut ,
            IDevice                                device ,
            [ Freeze ] IDeskCharacteristicsCreator creator )
        {
            sut.Initialize ( device ) ;

            creator.Received ( )
                   .Create ( sut ,
                             device ) ;
        }

        [ AutoDataTestMethod ]
        public void Initialize_ForDeviceIsNull_Throws (
            DeskCharacteristics sut )
        {
            Action action = ( ) => sut.Initialize ( null! ) ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "device" ) ;
        }

        [ AutoDataTestMethod ]
        public void WithCharacteristics_ForCharacteristicIsNull_Throws (
            DeskCharacteristics sut )
        {
            Action action = ( ) => sut.WithCharacteristics ( DeskCharacteristicKey.GenericAccess ,
                                                             null! ) ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "characteristic" ) ;
        }

        [ AutoDataTestMethod ]
        public void WithCharacteristics_ForCharacteristic_AddsCharacteristic (
            DeskCharacteristics sut ,
            IGenericAccess      genericAccess )
        {
            sut.WithCharacteristics ( DeskCharacteristicKey.GenericAccess ,
                                      genericAccess ) ;

            sut.Characteristics
               .Should ( )
               .Contain ( x => x.Key   == DeskCharacteristicKey.GenericAccess &&
                               x.Value == genericAccess ) ;
        }

        [ AutoDataTestMethod ]
        public void ToString_ForInvoked_Instance (
            DeskCharacteristics sut ,
            IGenericAccess      genericAccess ,
            IGenericAttribute   genericAttribute )
        {
            sut.WithCharacteristics ( DeskCharacteristicKey.GenericAccess ,
                                      genericAccess ) ;
            sut.WithCharacteristics ( DeskCharacteristicKey.GenericAttribute ,
                                      genericAttribute ) ;

            using var scope = new AssertionScope ( ) ;

            sut.ToString ( )
               .Should ( )
               .Contain ( "GenericAccess" ) ;

            sut.ToString ( )
               .Should ( )
               .Contain ( "GenericAttribute" ) ;
        }
    }
}