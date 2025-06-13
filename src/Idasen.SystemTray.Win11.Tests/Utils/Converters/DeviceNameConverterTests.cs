using FluentAssertions;
using Idasen.SystemTray.Win11.Utils;
using Idasen.SystemTray.Win11.Utils.Converters;

namespace Idasen.SystemTray.Win11.Tests.Utils.Converters ;

public class DeviceNameConverterTests
{
    [ Fact ]
    public void DefaultIfEmpty_ShouldReturnDefaultDeviceName_WhenDeviceNameIsNullOrWhiteSpace ( )
    {
        // Arrange
        var deviceName = "   " ;

        // Act
        var result = CreateSut ( ).DefaultIfEmpty ( deviceName ) ;

        // Assert
        result.Should ( ).Be ( Constants.DefaultDeviceName ) ;
    }

    [ Fact ]
    public void DefaultIfEmpty_ShouldReturnDeviceName_WhenDeviceNameIsNotNullOrWhiteSpace ( )
    {
        // Arrange
        var deviceName = "ValidDeviceName" ;

        // Act
        var result = CreateSut ( ).DefaultIfEmpty ( deviceName ) ;

        // Assert
        result.Should ( ).Be ( deviceName ) ;
    }

    [ Fact ]
    public void EmptyIfDefault_ShouldReturnEmptyString_WhenDeviceNameIsDefaultDeviceName ( )
    {
        // Arrange
        var deviceName = Constants.DefaultDeviceName ;

        // Act
        var result = CreateSut ( ).EmptyIfDefault ( deviceName ) ;

        // Assert
        result.Should ( ).Be ( string.Empty ) ;
    }

    [ Fact ]
    public void EmptyIfDefault_ShouldReturnDeviceName_WhenDeviceNameIsNotDefaultDeviceName ( )
    {
        // Arrange
        var deviceName = "AnotherDeviceName" ;

        // Act
        var result = CreateSut ( ).EmptyIfDefault ( deviceName ) ;

        // Assert
        result.Should ( ).Be ( deviceName ) ;
    }

    private static DeviceNameConverter CreateSut ( )
    {
        return new DeviceNameConverter ( ) ;
    }
}