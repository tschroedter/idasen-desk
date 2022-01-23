using System.Linq ;
using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [TestClass]
    public class DeskHeightMonitorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            _logger                = Substitute.For<ILogger>();
        }

        private DeskHeightMonitor CreateSut()
        {
            return new DeskHeightMonitor(_logger);
        }

        [DataTestMethod]
        [DataRow( "0",                   true)]
        [DataRow( "0,1",                 true)]
        [DataRow( "0,1,2",               true)]
        [DataRow( "0,1,2,3,4",           true)]
        [DataRow( "0,1,2,3,4,5",         true)]
        [DataRow( "0,1,2,3,4,5,6",       true)]
        [DataRow( "0,1,2,3,4,5,6,7",     true)]
        [DataRow( "0,1,2,3,4,5,6,7,8",   true)]
        [DataRow( "0,1,2,3,4,5,6,7,8,9", true)]
        [DataRow( "0,1,2,3,4,5,6,7,9,9", true)]
        [DataRow( "0,1,2,3,4,5,6,9,9,9", true)]
        [DataRow( "0,1,2,3,4,5,9,9,9,9", true)]
        [DataRow( "0,1,2,3,4,9,9,9,9,9", false)]
        [DataRow( "0,1,2,3,9,9,9,9,9,9", false)]
        [DataRow( "0,1,2,9,9,9,9,9,9,9", false)]
        [DataRow( "0,1,9,9,9,9,9,9,9,9", false)]
        [DataRow( "0,9,9,9,9,9,9,9,9,9", false)]
        [DataRow( "9,9,9,9,9,9,9,9,9,9", false)]
        [DataRow( "9,9,9,9,9",           false)]
        [DataRow( "9,9,9,9",             true)]
        [DataRow( "9,9,9",               true)]
        [DataRow( "9,9",                 true)]
        [DataRow( "9",                   true)]
        public void IsHeightChanging_ForValues_Expected(string values,
                                                        bool expected)
        {
            var heights = values.Split(',')
                                .Select(uint.Parse)
                                .ToArray();

            var sut = CreateSut ( ) ;

            foreach ( var height in heights)
            {
                sut.AddHeight ( height );
            }

            sut.IsHeightChanging ( )
               .Should ( )
               .Be (expected) ;
        }

        [TestMethod]
        public void Reset_ForInvoked_ResetsValues()
        {
            var sut = CreateSut();

            PopulateWithSameHeight ( sut ) ;

            sut.Reset (  );

            sut.IsHeightChanging()
               .Should()
               .BeTrue();
        }

        private static void PopulateWithSameHeight ( IDeskHeightMonitor sut )
        {
            for ( var i = 0 ; i < 10 ; i ++ )
            {
                sut.AddHeight ( 9 ) ;
            }
        }

        private ILogger                               _logger;
    }
}