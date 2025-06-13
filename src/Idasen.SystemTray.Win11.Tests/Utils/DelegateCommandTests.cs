using FluentAssertions;
using Idasen.SystemTray.Win11.Utils;
using NSubstitute;

namespace Idasen.SystemTray.Win11.Tests.Utils
{
    public class DelegateCommandTests
    {
        private readonly Action        _commandAction  = Substitute.For<Action>() ;
        private readonly Func < bool > _canExecuteFunc = Substitute.For<Func<bool>>() ;

        [Fact]
        public void Execute_ShouldInvokeCommandAction()
        {
            // Arrange  
            _commandAction.ClearReceivedCalls (  );
            _canExecuteFunc.Invoke().Returns(true);

            // Act  
            CreateSut().Execute(null);

            // Assert  
            _commandAction.Received(1).Invoke();
        }

        [Fact]
        public void CanExecute_ShouldReturnTrue_WhenCanExecuteFuncReturnsTrue()
        {
            // Arrange  
            _canExecuteFunc.Invoke().Returns(true);

            // Act  
            var result = CreateSut().CanExecute(null);

            // Assert  
            result.Should().BeTrue();
        }

        [Fact]
        public void CanExecute_ShouldReturnFalse_WhenCanExecuteFuncReturnsFalse()
        {
            // Arrange  
            _canExecuteFunc.Invoke().Returns(false);

            // Act  
            var result = CreateSut().CanExecute(null);

            // Assert  
            result.Should().BeFalse();
        }

        private DelegateCommand CreateSut ( )
        {
            return new DelegateCommand(_commandAction, _canExecuteFunc) ;
        }
    }
}
