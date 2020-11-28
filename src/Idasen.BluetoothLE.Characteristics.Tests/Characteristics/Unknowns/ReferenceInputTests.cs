using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [ TestClass ]
    public class ReferenceInputTests
    {
        [ TestMethod ]
        public void Ctrl1_ForInvoked_Empty ( )
        {
            CreateSut ( ).Ctrl1
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        private ReferenceInput CreateSut ( )
        {
            return new ReferenceInput ( ) ;
        }
    }
}