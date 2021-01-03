using System ;
using System.Collections.Generic ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Customs ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Customs
{
    [ TestClass ]
    public class GattCharacteristicProviderTests
    {
        private const string Description1 = "Description1" ;
        private const string Description2 = "Description2" ;

        [ TestInitialize ]
        public void Initialize ( )
        {
            _logger              = Substitute.For < ILogger > ( ) ;
            _gattCharacteristics = Substitute.For < IGattCharacteristicsResultWrapper > ( ) ;

            _foundCharacteristicsDictionary = new Dictionary < string , Guid > ( ) ;
            _customs                        = _foundCharacteristicsDictionary ;

            _customCharacteristic1 = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            _customCharacteristic1.CharacteristicProperties
                                  .Returns ( GattCharacteristicProperties.None ) ;

            _characteristic1 = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            _characteristic1.Uuid
                            .Returns ( _characteristics1Uuid ) ;

            _characteristic2 = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            _characteristic2.Uuid
                            .Returns ( _characteristics2Uuid ) ;


            _gattCharacteristics.Characteristics
                                .Returns ( _expectedCharacteristics ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryIsEmpty_CharacteristicsEmpty ( )
        {
            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            sut.Characteristics
               .Should ( )
               .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryIsEmpty_UnavailableCharacteristicsEmpty ( )
        {
            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            sut.UnavailableCharacteristics
               .Should ( )
               .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryWithSingleCustomCharacteristics_AddsCharacteristic ( )
        {
            _expectedCharacteristics.Add ( _characteristic1 ) ;

            _foundCharacteristicsDictionary.Add ( Description1 ,
                                                  _characteristics1Uuid ) ;

            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            using var scope = new AssertionScope ( ) ;

            sut.Characteristics
               .Count
               .Should ( )
               .Be ( 1 ,
                     "Characteristics Count" ) ;

            sut.Characteristics [ Description1 ]
               .Should ( )
               .Be ( _characteristic1 ,
                     "Contains Characteristic1" ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryWithSingleCustomCharacteristics_AddsProperties ( )
        {
            _expectedCharacteristics.Add ( _characteristic1 ) ;

            _foundCharacteristicsDictionary.Add ( Description1 ,
                                                  _characteristics1Uuid ) ;

            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            sut.Properties [ Description1 ]
               .Should ( )
               .Be ( _characteristic1.CharacteristicProperties ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryWithSingleCustomCharacteristics_UnavailableCharacteristics ( )
        {
            _expectedCharacteristics.Add ( _characteristic1 ) ;

            _foundCharacteristicsDictionary.Add ( Description1 ,
                                                  _characteristics1Uuid ) ;

            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            sut.UnavailableCharacteristics
               .Count
               .Should ( )
               .Be ( 0 ,
                     "Unavailable Characteristics should be empty" ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryWithMultipleCustomCharacteristics_AddsCharacteristic ( )
        {
            _expectedCharacteristics.Add ( _characteristic1 ) ;
            _expectedCharacteristics.Add ( _characteristic2 ) ;

            _foundCharacteristicsDictionary.Add ( Description1 ,
                                                  _characteristics1Uuid ) ;
            _foundCharacteristicsDictionary.Add ( Description2 ,
                                                  _characteristics2Uuid ) ;

            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            using var scope = new AssertionScope ( ) ;

            sut.Characteristics
               .Count
               .Should ( )
               .Be ( 2 ,
                     "Characteristics Count" ) ;

            sut.Characteristics [ Description1 ]
               .Should ( )
               .Be ( _characteristic1 ,
                     "Contains Characteristic1" ) ;

            sut.Characteristics [ Description2 ]
               .Should ( )
               .Be ( _characteristic2 ,
                     "Contains Characteristic2" ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryWithMultipleCustomCharacteristics_AddsProperties ( )
        {
            _expectedCharacteristics.Add ( _characteristic1 ) ;
            _expectedCharacteristics.Add ( _characteristic2 ) ;

            _foundCharacteristicsDictionary.Add ( Description1 ,
                                                  _characteristics1Uuid ) ;
            _foundCharacteristicsDictionary.Add ( Description2 ,
                                                  _characteristics2Uuid ) ;

            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            using var scope = new AssertionScope ( ) ;

            sut.Properties [ Description1 ]
               .Should ( )
               .Be ( _characteristic1.CharacteristicProperties ,
                     "Characteristic1" ) ;

            sut.Properties [ Description2 ]
               .Should ( )
               .Be ( _characteristic2.CharacteristicProperties ,
                     "Characteristic2" ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryWithMultipleCustomCharacteristics_UnavailableCharacteristics ( )
        {
            _expectedCharacteristics.Add ( _characteristic1 ) ;
            _expectedCharacteristics.Add ( _characteristic2 ) ;

            _foundCharacteristicsDictionary.Add ( Description1 ,
                                                  _characteristics1Uuid ) ;
            _foundCharacteristicsDictionary.Add ( Description2 ,
                                                  _characteristics2Uuid ) ;

            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            sut.UnavailableCharacteristics
               .Count
               .Should ( )
               .Be ( 0 ) ;
        }

        [ TestMethod ]
        public void Refresh_ForReadOnlyDictionaryNotContainingExpectedCharacteristics_UnavailableCharacteristics ( )
        {
            _foundCharacteristicsDictionary.Add ( Description1 ,
                                                  _characteristics1Uuid ) ;

            var sut = CreateSut ( ) ;

            sut.Refresh ( _customs ) ;

            sut.UnavailableCharacteristics
               .Count
               .Should ( )
               .Be ( 1 ) ;
        }

        private GattCharacteristicProvider CreateSut ( )
        {
            return new GattCharacteristicProvider ( _logger ,
                                                    _gattCharacteristics ) ;
        }

        private readonly Guid _characteristics1Uuid = Guid.NewGuid ( ) ;
        private readonly Guid _characteristics2Uuid = Guid.NewGuid ( ) ;

        private readonly List < IGattCharacteristicWrapper > _expectedCharacteristics =
            new List < IGattCharacteristicWrapper > ( ) ;

        private IGattCharacteristicWrapper            _characteristic1 ;
        private IGattCharacteristicWrapper            _characteristic2 ;
        private IGattCharacteristicWrapper            _customCharacteristic1 ;
        private IReadOnlyDictionary < string , Guid > _customs ;
        private Dictionary < string , Guid >          _foundCharacteristicsDictionary ;
        private IGattCharacteristicsResultWrapper     _gattCharacteristics ;
        private ILogger                               _logger ;
    }
}