using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Utils.Converters ;
using NSubstitute ;

namespace Idasen.SystemTray.Win11.Tests.Utils.Converters ;

public class DeviceAddressToULongConverterTests
{
    private readonly DeviceAddressToULongConverter _converter ;
    private readonly IStringToUIntConverter        _stringToUIntConverter ;

    public DeviceAddressToULongConverterTests ( )
    {
        _stringToUIntConverter = Substitute.For < IStringToUIntConverter > ( ) ;
        _converter             = new DeviceAddressToULongConverter ( _stringToUIntConverter ) ;
    }

    [ Fact ]
    public void DefaultIfEmpty_ShouldReturnDefault_WhenDeviceAddressIsNullOrWhiteSpace ( )
    {
        // Arrange
        const string deviceAddress = " " ;
        const ulong  expected      = Constants.DefaultDeviceAddress ;

        // Act
        var result = _converter.DefaultIfEmpty ( deviceAddress ) ;

        // Assert
        result.Should ( )
              .Be ( expected ) ;
    }

    [ Fact ]
    public void DefaultIfEmpty_ShouldCallConvertStringToUlongOrDefault_WhenDeviceAddressIsNotNullOrWhiteSpace ( )
    {
        // Arrange
        const string deviceAddress = "12345" ;
        const ulong  defaultValue  = Constants.DefaultDeviceAddress ;
        const ulong  expected      = 12345 ;

        _stringToUIntConverter.ConvertStringToUlongOrDefault ( deviceAddress ,
                                                               defaultValue ).Returns ( expected ) ;

        // Act
        var result = _converter.DefaultIfEmpty ( deviceAddress ) ;

        // Assert
        result.Should ( )
              .Be ( expected ) ;
        _stringToUIntConverter.Received ( 1 )
                              .ConvertStringToUlongOrDefault ( deviceAddress ,
                                                               defaultValue ) ;
    }

    [ Fact ]
    public void EmptyIfDefault_ShouldReturnEmptyString_WhenDeviceAddressIsDefault ( )
    {
        // Arrange
        const ulong deviceAddress = Constants.DefaultDeviceAddress ;

        // Act
        var result = _converter.EmptyIfDefault ( deviceAddress ) ;

        // Assert
        result.Should ( )
              .Be ( string.Empty ) ;
    }

    [ Fact ]
    public void EmptyIfDefault_ShouldReturnDeviceAddressAsString_WhenDeviceAddressIsNotDefault ( )
    {
        // Arrange
        const ulong deviceAddress = 12345 ;

        // Act
        var result = _converter.EmptyIfDefault ( deviceAddress ) ;

        // Assert
        result.Should ( )
              .Be ( deviceAddress.ToString ( ) ) ;
    }
}