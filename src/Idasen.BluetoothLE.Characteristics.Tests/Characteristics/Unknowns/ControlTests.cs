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
        public void RawControl2_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawControl2
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawControl3_ForInvoked_Empty ( )
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