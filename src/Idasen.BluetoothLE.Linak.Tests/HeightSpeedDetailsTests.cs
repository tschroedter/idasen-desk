using System ;
using FluentAssertions ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class HeightSpeedDetailsTests
    {
        [ AutoDataTestMethod ]
        public void Timestamp_ForInvoked_Instance (
            HeightSpeedDetails        sut ,
            [ Freeze ] DateTimeOffset timestamp )
        {
            sut.Timestamp
               .Should ( )
               .Be ( timestamp ) ;
        }

        [ AutoDataTestMethod ]
        public void Height_ForInvoked_Instance (
            HeightSpeedDetails sut ,
            [ Freeze ] uint    height )
        {
            sut.Height
               .Should ( )
               .Be ( height ) ;
        }

        [ AutoDataTestMethod ]
        public void Speed_ForInvoked_Instance (
            HeightSpeedDetails sut ,
            [ Freeze ] int     speed )
        {
            sut.Speed
               .Should ( )
               .Be ( speed ) ;
        }

        [ AutoDataTestMethod ]
        public void ToString_ForInvoked_Instance (
            HeightSpeedDetails        sut ,
            [ Freeze ] DateTimeOffset timestamp ,
            [ Freeze ] uint           height ,
            [ Freeze ] int            speed )
        {
            var expected = $"Timestamp = {timestamp:O}, " +
                           $"Height = {height}, "         +
                           $"Speed = {speed}" ;

            sut.ToString ( )
               .Should ( )
               .Be ( expected ) ;
        }
    }
}