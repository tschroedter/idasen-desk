using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class DelegateCommandTests
{
    private readonly Func < object ? , bool > _canExecuteFunc = Substitute.For < Func < object ? , bool > > ( ) ;
    private readonly Action < object ? >      _commandAction  = Substitute.For < Action < object ? > > ( ) ;

    [ Fact ]
    public void Execute_ShouldInvokeCommandAction ( )
    {
        // Arrange  
        _commandAction.ClearReceivedCalls ( ) ;
        _canExecuteFunc.Invoke ( null ).Returns ( true ) ;

        // Act  
        CreateSut ( ).Execute ( null ) ;

        // Assert  
        _commandAction.Received ( 1 ).Invoke ( null ) ;
    }

    [ Fact ]
    public void CanExecute_ShouldReturnTrue_WhenCanExecuteFuncReturnsTrue ( )
    {
        // Arrange  
        _canExecuteFunc.Invoke ( null ).Returns ( true ) ;

        // Act  
        var result = CreateSut ( ).CanExecute ( null ) ;

        // Assert  
        result.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void CanExecute_ShouldReturnFalse_WhenCanExecuteFuncReturnsFalse ( )
    {
        // Arrange  
        _canExecuteFunc.Invoke ( null ).Returns ( false ) ;

        // Act  
        var result = CreateSut ( ).CanExecute ( null ) ;

        // Assert  
        result.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void CanExecute_ShouldForwardParameterToCanExecuteFunc ( )
    {
        // Arrange
        var parameter = new object ( ) ;
        _canExecuteFunc.Invoke ( parameter ).Returns ( true ) ;
        var sut = CreateSut ( ) ;

        // Act
        sut.CanExecute ( parameter ) ;

        // Assert
        _canExecuteFunc.Received ( 1 ).Invoke ( parameter ) ;
    }

    private DelegateCommand CreateSut ( )
    {
        return new DelegateCommand ( _commandAction ,
                                     _canExecuteFunc ) ;
    }
}