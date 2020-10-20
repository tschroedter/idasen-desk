using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [ TestClass ]
    public class DpgTests
    {
        [ TestMethod ]
        public void Constructor_ForInvoked_SetsRawDpg ( )
        {
            CreateSut ( ).RawDpg
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        private Dpg CreateSut ( )
        {
            return new Dpg ( ) ;
        }
    }
}