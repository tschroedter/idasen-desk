using System ;
using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [ TestClass ]
    public class ControlTests
    {
        [ TestMethod ]
        public void Constructor_ForInvoked_SetsRawControl2 ( )
        {
            CreateSut ( ).RawControl2
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsRawControl3 ( )
        {
            CreateSut ( ).RawControl3
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public async Task TryWriteRawControl2_ForInvoked_ReturnsFalse ( )
        {
            var result = await CreateSut ( ).TryWriteRawControl2 ( Array.Empty < byte > ( ) ) ;

            result.Should ( )
                  .BeFalse ( ) ;
        }

        private Control CreateSut ( )
        {
            return new Control ( ) ;
        }
    }
}