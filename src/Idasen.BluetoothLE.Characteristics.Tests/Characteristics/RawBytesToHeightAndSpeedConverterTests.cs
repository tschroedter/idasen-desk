using System.Collections.Generic ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Selkie.AutoMocking ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ AutoDataTestClass ]
    public class RawBytesToHeightAndSpeedConverterTests
    {
        [ TestInitialize ]
        public void Initialize ( )
        {
            _logger = Substitute.For < ILogger > ( ) ;
        }

        [ DataTestMethod ]
        [ DynamicData ( nameof ( GetData ) ,
                        DynamicDataSourceType.Method ) ]
        public void TryConvert_ForBytes_Converts (
            IEnumerable < byte > bytes ,
            bool                 expectedResult ,
            uint                 expectedHeight ,
            int                  expectedSpeed )
        {
            using var assertionScope = new AssertionScope ( ) ;

            CreateSut ( ).TryConvert ( bytes ,
                                       out var height ,
                                       out var speed )
                         .Should ( )
                         .Be ( expectedResult ,
                               "Result" ) ;

            height.Should ( ).Be ( expectedHeight ,
                                   "Height" ) ;
            speed.Should ( ).Be ( expectedSpeed ,
                                  "Speed" ) ;
        }

        public static IEnumerable < object [ ] > GetData ( )
        {
            yield return new object [ ]
                         {
                             new byte [ ] { } ,
                             false ,
                             0u ,
                             0
                         } ;

            yield return new object [ ]
                         {
                             new byte [ ] { 1 } ,
                             false ,
                             0u ,
                             0
                         } ;

            yield return new object [ ]
                         {
                             new byte [ ] { 1 , 2 } ,
                             false ,
                             0u ,
                             0
                         } ;

            yield return new object [ ]
                         {
                             new byte [ ] { 1 , 2 , 3 } ,
                             false ,
                             0u ,
                             0
                         } ;

            yield return new object [ ]
                         {
                             new byte [ ] { 1 , 2 , 3 , 4 } ,
                             true ,
                             6713u ,
                             1027
                         } ;

            yield return new object [ ]
                         {
                             new byte [ ] { 1 , 2 , 3 , 4 , 5 } ,
                             true ,
                             6713u ,
                             1027
                         } ;
        }

        private RawBytesToHeightAndSpeedConverter CreateSut ( )
        {
            return new RawBytesToHeightAndSpeedConverter ( _logger ) ;
        }

        private ILogger _logger ;
    }
}