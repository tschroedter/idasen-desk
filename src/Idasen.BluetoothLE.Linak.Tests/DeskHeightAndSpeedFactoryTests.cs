using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ TestClass ]
    public class DeskHeightAndSpeedFactoryTests
    {
        [ TestInitialize ]
        public void Initialize ( )
        {
            _referenceOutput = Substitute.For < IReferenceOutput > ( ) ;
            _factory         = TestFactory ;
        }

        [ TestMethod ]
        public void Create_ForReferenceOutputNull_Throws ( )
        {
            Action action = ( ) => { CreateSut ( ).Create ( null! ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "referenceOutput" ) ;
        }

        [ TestMethod ]
        public void Create_ForInvoked_ReturnsInstance ( )
        {
            CreateSut ( ).Create ( _referenceOutput )
                         .Should ( )
                         .NotBeNull ( ) ;
        }

        private IDeskHeightAndSpeed TestFactory ( IReferenceOutput _ )
        {
            return Substitute.For < IDeskHeightAndSpeed > ( ) ;
        }

        private DeskHeightAndSpeedFactory CreateSut ( )
        {
            return new DeskHeightAndSpeedFactory ( _factory ) ;
        }

        private DeskHeightAndSpeed.Factory _factory ;
        private IReferenceOutput           _referenceOutput ;
    }
}