using FluentAssertions ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery
{
    [ TestClass ]
    public class DeviceFactoryTests
    {
        [ TestInitialize ]
        public void Initialize ( )
        {
            _deviceFactory  = Substitute.For < Device.Factory > ( ) ;
            _wrapperFactory = Substitute.For < IBluetoothLeDeviceWrapperFactory > ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_Instance ( )
        {
            CreateSut ( ).Should ( )
                         .NotBeNull ( ) ;
        }

        private DeviceFactory CreateSut ( )
        {
            return new DeviceFactory ( _deviceFactory ,
                                       _wrapperFactory ) ;
        }

        private Device.Factory                   _deviceFactory ;
        private IBluetoothLeDeviceWrapperFactory _wrapperFactory ;
    }
}