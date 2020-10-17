using Idasen.BluetoothLE.Linak.Interfaces;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Selkie.AutoMocking;
using Serilog;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [AutoDataTestClass]
    public class DeskMoverTests
    {
        private       ILogger              _logger;
        private       TestScheduler        _scheduler;
        private       IDeskCommandExecutor _executor;
        private const uint                 DefaultHeight = 1000;
        private const int                  DefaultSpeed  = 200;

        [TestInitialize]
        public void Initialize()
        {
            _logger         = Substitute.For<ILogger>();
            _scheduler      = new TestScheduler();
            _executor       = Substitute.For<IDeskCommandExecutor>();
        }

        // todo more tests
    }
}
