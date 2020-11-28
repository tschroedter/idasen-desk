using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [ TestClass ]
    public class GenericAttributeTests
    {
        [ TestMethod ]
        public void RawServiceChanged_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawServiceChanged
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        private GenericAttribute CreateSut ( )
        {
            return new GenericAttribute ( ) ;
        }
    }
}