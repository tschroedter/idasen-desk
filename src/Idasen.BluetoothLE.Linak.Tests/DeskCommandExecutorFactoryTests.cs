using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ TestClass ]
    public class DeskCommandExecutorFactoryTests
    {
        [ TestInitialize ]
        public void Initialize ( )
        {
            _factory = TestFactory ;

            _control = Substitute.For < IControl > ( ) ;
        }

        private IDeskCommandExecutor TestFactory ( IControl executor )
        {
            return Substitute.For < IDeskCommandExecutor > ( ) ;
        }

        [ TestMethod ]
        public void Create_ForControlNull_Throws ( )
        {
            Action action = ( ) => { CreateSut ( ).Create ( null! ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "control" ) ;
        }

        [ TestMethod ]
        public void CreateForInvoked_ReturnsInstance ( )
        {
            CreateSut ( ).Create ( _control )
                         .Should ( )
                         .NotBeNull ( ) ;
        }

        private DeskCommandExecutorFactory CreateSut ( )
        {
            return new DeskCommandExecutorFactory ( _factory ) ;
        }

        private IControl                    _control ;
        private DeskCommandExecutor.Factory _factory ;
    }
}