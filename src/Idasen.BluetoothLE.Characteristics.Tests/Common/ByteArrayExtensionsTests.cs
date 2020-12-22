using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common
{
    [ TestClass ]
    public class ByteArrayExtensionsTests
    {
        [ TestMethod ]
        public void ToHex_ForBytesNull_Throws ( )
        {
            byte [ ] bytes = null ;

            // ReSharper disable once AssignNullToNotNullAttribute
            Action action = ( ) => { bytes.ToHex ( ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "array" ) ;
        }

        [ TestMethod ]
        public void ToHex_ForBytes_Instance ( )
        {
            var bytes = new byte [ ] { 1 , 2 , 3 } ;

            bytes.ToHex ( )
                 .Should ( )
                 .Be ( "01-02-03" ) ;
        }
    }
}