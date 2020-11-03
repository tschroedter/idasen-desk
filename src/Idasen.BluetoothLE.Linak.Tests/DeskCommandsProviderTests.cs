using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Control ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ TestClass ]
    public class DeskCommandsProviderTests
    {
        [ TestMethod ]
        public void TryGetValue_ForDeskCommandsMoveDown_ReturnsTrue ( )
        {
            CreateSut ( ).TryGetValue ( DeskCommands.MoveDown ,
                                        out var _ )
                         .Should ( )
                         .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void TryGetValue_ForDeskCommandsMoveDown_ReturnsBytes ( )
        {
            var expected = new byte [ ] { 0x46 , 0x00 } ;

            CreateSut ( ).TryGetValue ( DeskCommands.MoveDown ,
                                        out var bytes ) ;

            bytes.Should ( )
                 .BeEquivalentTo ( expected ) ;
        }

        [ TestMethod ]
        public void TryGetValue_ForDeskCommandsMoveUp_ReturnsTrue ( )
        {
            CreateSut ( ).TryGetValue ( DeskCommands.MoveUp ,
                                        out var _ )
                         .Should ( )
                         .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void TryGetValue_ForDeskCommandsMoveUp_ReturnsBytes ( )
        {
            var expected = new byte [ ] { 0x47 , 0x00 } ;

            CreateSut ( ).TryGetValue ( DeskCommands.MoveUp ,
                                        out var bytes ) ;

            bytes.Should ( )
                 .BeEquivalentTo ( expected ) ;
        }

        [ TestMethod ]
        public void TryGetValue_ForDeskCommandsMoveStop_ReturnsTrue ( )
        {
            CreateSut ( ).TryGetValue ( DeskCommands.MoveStop ,
                                        out var _ )
                         .Should ( )
                         .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void TryGetValue_ForDeskCommandsMoveStop_ReturnsBytes ( )
        {
            var expected = new byte [ ] { 0x48 , 0x00 } ;

            CreateSut ( ).TryGetValue ( DeskCommands.MoveStop ,
                                        out var bytes ) ;

            bytes.Should ( )
                 .BeEquivalentTo ( expected ) ;
        }

        private DeskCommandsProvider CreateSut ( )
        {
            return new DeskCommandsProvider ( ) ;
        }
    }
}