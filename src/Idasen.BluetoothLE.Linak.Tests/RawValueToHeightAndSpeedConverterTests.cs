using FluentAssertions ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class RawValueToHeightAndSpeedConverterTests
    {
        [ AutoDataTestMethod ]
        public void TryConvert_ForValidBytes_ReturnsTrue (
            RawValueToHeightAndSpeedConverter sut )
        {
            sut.TryConvert ( _validHeightAndSpeed ,
                             out var _ ,
                             out var _ )
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void TryConvert_ForValidBytes_ReturnsHeight (
            RawValueToHeightAndSpeedConverter sut )
        {
            var expected = RawValueToHeightAndSpeedConverter.HeightBaseInMicroMeter + 1u ;

            sut.TryConvert ( _validHeightAndSpeed ,
                             out var height ,
                             out var _ ) ;

            height.Should ( )
                  .Be ( expected ) ;
        }

        [ AutoDataTestMethod ]
        public void TryConvert_ForValidBytes_ReturnsSpeed (
            RawValueToHeightAndSpeedConverter sut )
        {
            sut.TryConvert ( _validHeightAndSpeed ,
                             out var _ ,
                             out var speed ) ;

            speed.Should ( )
                 .Be ( 2 ) ;
        }

        [ AutoDataTestMethod ]
        public void TryConvert_ForInvalidBytes_ReturnsFalse (
            RawValueToHeightAndSpeedConverter sut )
        {
            sut.TryConvert ( _invalidHeightAndSpeed ,
                             out var _ ,
                             out var _ )
               .Should ( )
               .BeFalse ( ) ;
        }

        private readonly byte [ ] _invalidHeightAndSpeed = { 1 , 0 , 2 } ;
        private readonly byte [ ] _validHeightAndSpeed   = { 1 , 0 , 2 , 0 } ;
    }
}