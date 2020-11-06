using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class DeskCharacteristicsCreatorTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForInvokedWithCharacteristics_AddsGenericAccess (
            DeskCharacteristicsCreator sut ,
            IDeskCharacteristics       characteristics ,
            IDevice                    device ,
            [ Freeze ] IGenericAccess  characteristic )
        {
            characteristics.WithCharacteristics ( Arg.Any < DeskCharacteristicKey > ( ) ,
                                                  Arg.Any < ICharacteristicBase > ( ) )
                           .Returns ( characteristics ) ;

            sut.Create ( characteristics ,
                         device ) ;

            characteristics.Received ( )
                           .WithCharacteristics ( DeskCharacteristicKey.GenericAccess ,
                                                  characteristic ) ;
        }

        [ AutoDataTestMethod ]
        public void Create_ForInvokedWithCharacteristics_AddsGenericAttribute (
            DeskCharacteristicsCreator   sut ,
            IDeskCharacteristics         characteristics ,
            IDevice                      device ,
            [ Freeze ] IGenericAttribute characteristic )
        {
            characteristics.WithCharacteristics ( Arg.Any < DeskCharacteristicKey > ( ) ,
                                                  Arg.Any < ICharacteristicBase > ( ) )
                           .Returns ( characteristics ) ;

            sut.Create ( characteristics ,
                         device ) ;

            characteristics.Received ( )
                           .WithCharacteristics ( DeskCharacteristicKey.GenericAttribute ,
                                                  characteristic ) ;
        }

        [ AutoDataTestMethod ]
        public void Create_ForInvokedWithCharacteristics_AddsReferenceInput (
            DeskCharacteristicsCreator sut ,
            IDeskCharacteristics       characteristics ,
            IDevice                    device ,
            [ Freeze ] IReferenceInput characteristic )
        {
            characteristics.WithCharacteristics ( Arg.Any < DeskCharacteristicKey > ( ) ,
                                                  Arg.Any < ICharacteristicBase > ( ) )
                           .Returns ( characteristics ) ;

            sut.Create ( characteristics ,
                         device ) ;

            characteristics.Received ( )
                           .WithCharacteristics ( DeskCharacteristicKey.ReferenceInput ,
                                                  characteristic ) ;
        }

        [ AutoDataTestMethod ]
        public void Create_ForInvokedWithCharacteristics_AddsReferenceOutput (
            DeskCharacteristicsCreator  sut ,
            IDeskCharacteristics        characteristics ,
            IDevice                     device ,
            [ Freeze ] IReferenceOutput characteristic )
        {
            characteristics.WithCharacteristics ( Arg.Any < DeskCharacteristicKey > ( ) ,
                                                  Arg.Any < ICharacteristicBase > ( ) )
                           .Returns ( characteristics ) ;

            sut.Create ( characteristics ,
                         device ) ;

            characteristics.Received ( )
                           .WithCharacteristics ( DeskCharacteristicKey.ReferenceOutput ,
                                                  characteristic ) ;
        }

        [ AutoDataTestMethod ]
        public void Create_ForInvokedWithCharacteristics_AddsDpg (
            DeskCharacteristicsCreator sut ,
            IDeskCharacteristics       characteristics ,
            IDevice                    device ,
            [ Freeze ] IDpg            characteristic )
        {
            characteristics.WithCharacteristics ( Arg.Any < DeskCharacteristicKey > ( ) ,
                                                  Arg.Any < ICharacteristicBase > ( ) )
                           .Returns ( characteristics ) ;

            sut.Create ( characteristics ,
                         device ) ;

            characteristics.Received ( )
                           .WithCharacteristics ( DeskCharacteristicKey.Dpg ,
                                                  characteristic ) ;
        }

        [ AutoDataTestMethod ]
        public void Create_ForInvokedWithCharacteristics_AddsControl (
            DeskCharacteristicsCreator sut ,
            IDeskCharacteristics       characteristics ,
            IDevice                    device ,
            [ Freeze ] IControl        characteristic )
        {
            characteristics.WithCharacteristics ( Arg.Any < DeskCharacteristicKey > ( ) ,
                                                  Arg.Any < ICharacteristicBase > ( ) )
                           .Returns ( characteristics ) ;

            sut.Create ( characteristics ,
                         device ) ;

            characteristics.Received ( )
                           .WithCharacteristics ( DeskCharacteristicKey.Control ,
                                                  characteristic ) ;
        }
    }
}