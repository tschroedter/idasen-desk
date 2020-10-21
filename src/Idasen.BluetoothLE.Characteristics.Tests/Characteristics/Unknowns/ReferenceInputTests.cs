using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [TestClass]
    public class ReferenceInputTests
    {
        [TestMethod]
        public void Constructor_ForInvoked_SetsCtrl1()
        {
            CreateSut().Ctrl1
                       .Should()
                       .BeEmpty();
        }

        private ReferenceInput CreateSut()
        {
            return new ReferenceInput();
        }
    }
}