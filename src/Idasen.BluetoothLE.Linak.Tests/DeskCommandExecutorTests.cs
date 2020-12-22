using System ;
using System.Collections.Generic ;
using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using NSubstitute ;
using Selkie.AutoMocking ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class DeskCommandExecutorTests
    {
        [ AutoDataTestMethod ]
        public void Constructor_ForLoggerNull_Throws (
            Lazy < DeskCommandExecutor > sut ,
            [ BeNull ] ILogger           logger )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( logger ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForProviderNull_Throws (
            Lazy < DeskCommandExecutor >     sut ,
            [ BeNull ] IDeskCommandsProvider provider )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( provider ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForControlNull_Throws (
            Lazy < DeskCommandExecutor > sut ,
            [ BeNull ] IControl          control )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( control ) ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Up_ForInvokedWithUnknownCommand_ReturnsFalse (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider ,
            [ Freeze ] IControl              control )
        {
            provider.TryGetValue ( DeskCommands.MoveUp ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = null ;
                                   return false ;
                               } ) ;
            await sut.Up ( ) ;

            await control.DidNotReceive ( )
                         .TryWriteRawControl2 ( Arg.Any < IEnumerable < byte > > ( ) ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Up_ForInvoked_ReturnsTrueForSuccess (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider ,
            [ Freeze ] IControl              control )
        {
            var bytes = new byte [ ] { 0 , 1 } ;

            provider.TryGetValue ( DeskCommands.MoveUp ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = bytes ;
                                   return true ;
                               } ) ;
            var actual = await sut.Up ( ) ;

            actual.Should ( )
                  .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Up_ForInvoked_CallsControl (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider ,
            [ Freeze ] IControl              control )
        {
            var bytes = new byte [ ] { 0 , 1 } ;

            provider.TryGetValue ( DeskCommands.MoveUp ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = bytes ;
                                   return true ;
                               } ) ;
            await sut.Up ( ) ;

            await control.Received ( )
                         .TryWriteRawControl2 ( bytes ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Down_ForInvokedWithUnknownCommand_ReturnsFalse (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider ,
            [ Freeze ] IControl              control )
        {
            provider.TryGetValue ( DeskCommands.MoveDown ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = null ;
                                   return false ;
                               } ) ;
            await sut.Down ( ) ;

            await control.DidNotReceive ( )
                         .TryWriteRawControl2 ( Arg.Any < IEnumerable < byte > > ( ) ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Down_ForInvoked_ReturnsTrueForSuccess (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider )
        {
            var bytes = new byte [ ] { 0 , 1 } ;

            provider.TryGetValue ( DeskCommands.MoveDown ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = bytes ;
                                   return true ;
                               } ) ;
            var actual = await sut.Down ( ) ;

            actual.Should ( )
                  .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Down_ForInvoked_CallsControl (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider ,
            [ Freeze ] IControl              control )
        {
            var bytes = new byte [ ] { 0 , 1 } ;

            provider.TryGetValue ( DeskCommands.MoveDown ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = bytes ;
                                   return true ;
                               } ) ;
            await sut.Down ( ) ;

            await control.Received ( )
                         .TryWriteRawControl2 ( bytes ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Stop_ForInvokedWithUnknownCommand_ReturnsFalse (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider ,
            [ Freeze ] IControl              control )
        {
            provider.TryGetValue ( DeskCommands.MoveStop ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = null ;
                                   return false ;
                               } ) ;
            await sut.Stop ( ) ;

            await control.DidNotReceive ( )
                         .TryWriteRawControl2 ( Arg.Any < IEnumerable < byte > > ( ) ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Stop_ForInvoked_ReturnsTrueForSuccess (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider )
        {
            var bytes = new byte [ ] { 0 , 1 } ;

            provider.TryGetValue ( DeskCommands.MoveStop ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = bytes ;
                                   return true ;
                               } ) ;
            var actual = await sut.Stop ( ) ;

            actual.Should ( )
                  .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task Stop_ForInvoked_CallsControl (
            DeskCommandExecutor              sut ,
            [ Freeze ] IDeskCommandsProvider provider ,
            [ Freeze ] IControl              control )
        {
            var bytes = new byte [ ] { 0 , 1 } ;

            provider.TryGetValue ( DeskCommands.MoveStop ,
                                   out Arg.Any < IEnumerable < byte > > ( ) )
                    .Returns ( x =>
                               {
                                   x [ 1 ] = bytes ;
                                   return true ;
                               } ) ;
            await sut.Stop ( ) ;

            await control.Received ( )
                         .TryWriteRawControl2 ( bytes ) ;
        }
    }
}