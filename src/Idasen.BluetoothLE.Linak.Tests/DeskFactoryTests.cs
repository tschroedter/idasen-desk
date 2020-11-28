using System ;
using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ TestClass ]
    public class DeskFactoryTests
    {
        [ TestInitialize ]
        public void Initialize ( )
        {
            _deviceFactory        = Substitute.For < IDeviceFactory > ( ) ;
            _deskConnectorFactory = Substitute.For < Func < IDevice , IDeskConnector > > ( ) ;
            _deskFactory          = Substitute.For < Func < IDeskConnector , IDesk > > ( ) ;
        }

        [ TestMethod ]
        public async Task CreateAsync_ForInvoked_ReturnsInstance ( )
        {
            var actual = await CreateSut ( ).CreateAsync ( 1u ) ;

            actual.Should ( )
                  .NotBeNull ( ) ;
        }

        private DeskFactory CreateSut ( )
        {
            return new DeskFactory ( _deviceFactory ,
                                     _deskConnectorFactory ,
                                     _deskFactory ) ;
        }

        private Func < IDevice , IDeskConnector > _deskConnectorFactory ;
        private Func < IDeskConnector , IDesk >   _deskFactory ;
        private IDeviceFactory                    _deviceFactory ;
    }
}