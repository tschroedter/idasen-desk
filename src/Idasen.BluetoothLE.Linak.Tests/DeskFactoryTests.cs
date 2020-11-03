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
            _deviceFactory = Substitute.For < IDeviceFactory > ( ) ;
            _deskFactory   = Substitute.For < Func < IDevice , IDesk > > ( ) ;
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
                                     _deskFactory ) ;
        }

        private Func < IDevice , IDesk > _deskFactory ;
        private IDeviceFactory           _deviceFactory ;
    }
}