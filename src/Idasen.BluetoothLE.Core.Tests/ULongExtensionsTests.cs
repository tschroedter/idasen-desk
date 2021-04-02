using FluentAssertions ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests
{
    [ TestClass ]
    public class ULongExtensionsTests
    {
        [ TestMethod ]
        public void ToMacAddress_ForValidValue_ReturnsMacAddress ( )
        {
            const ulong value = 254682828386071 ;

            value.ToMacAddress ( )
                 .Should ( )
                 .Be ( "E7:A1:F7:84:2F:17" ) ;
        }

        [ TestMethod ]
        public void ToMacAddress_ForValueIsZero_ReturnsValue ( )
        {
            const ulong value = 0 ;

            value.ToMacAddress ( )
                 .Should ( )
                 .Be ( "0" ) ;
        }

        [ TestMethod ]
        public void ToMacAddress_ForValueNotLargeEnough_ReturnsValue ( )
        {
            const ulong value = 2546828283860 ;

            // ReSharper disable StringLiteralTypo
            value.ToMacAddress ( )
                 .Should ( )
                 .Be ( "250FACB8FD4" ) ;
            // ReSharper restore StringLiteralTypo
        }

        [ TestMethod ]
        public void ToMacAddress_ForValueToLarge_ReturnsInvalidMacAddress ( )
        {
            const ulong value = 25468282838607111 ;

            value.ToMacAddress ( )
                 .Should ( )
                 .Be ( "5A:7B:44:AF:A2:6507" ) ;
        }
    }
}