using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [ TestClass ]
    public class GenericAccessTests
    {
        [ TestMethod ]
        public void Constructor_ForInvoked_SetsGattServiceUuid ( )
        {
            CreateSut ( ).GattServiceUuid
                         .Should ( )
                         .Be ( Guid.Empty ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsRawResolution ( )
        {
            CreateSut ( ).RawResolution
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsRawParameters ( )
        {
            CreateSut ( ).RawParameters
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsRawAppearance ( )
        {
            CreateSut ( ).RawAppearance
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsRawDeviceName ( )
        {
            CreateSut ( ).RawDeviceName
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void AppearanceChanged_ForInvoked_Throws ( )
        {
            Action action = ( ) => CreateSut ( ).AppearanceChanged
                                                .Subscribe ( ) ;

            action.Should ( )
                  .Throw < NotInitializeException > ( ) ;
        }

        [ TestMethod ]
        public void ParametersChanged_ForInvoked_Throws ( )
        {
            Action action = ( ) => CreateSut ( ).ParametersChanged
                                                .Subscribe ( ) ;

            action.Should ( )
                  .Throw < NotInitializeException > ( ) ;
        }

        [ TestMethod ]
        public void ResolutionChanged_ForInvoked_Throws ( )
        {
            Action action = ( ) => CreateSut ( ).ResolutionChanged
                                                .Subscribe ( ) ;

            action.Should ( )
                  .Throw < NotInitializeException > ( ) ;
        }

        [ TestMethod ]
        public void DeviceNameChanged_ForInvoked_Throws ( )
        {
            Action action = ( ) => CreateSut ( ).DeviceNameChanged
                                                .Subscribe ( ) ;

            action.Should ( )
                  .Throw < NotInitializeException > ( ) ;
        }

        private GenericAccess CreateSut ( )
        {
            return new GenericAccess ( ) ;
        }
    }
}