using FluentAssertions;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;
using Idasen.BluetoothLE.Core.ServicesDiscovery;
using NSubstitute;
using Selkie.AutoMocking;

namespace Idasen.BluetoothLE.Tests.ServicesDiscovery
{
    [AutoDataTestClass]
    public class GattServicesDictionaryTests
    {
        [AutoDataTestMethod]
        public void Indexer_ForServiceAndResult_SetsKeyAndValue(
            GattServicesDictionary            sut,
            IGattDeviceServiceWrapper         service,
            IGattCharacteristicsResultWrapper result)
        {
            sut[service] = result;

            sut[service]
               .Should()
               .Be(result);
        }

        [AutoDataTestMethod]
        public void Indexer_ForServiceAndResult_UpdatesCount(
            GattServicesDictionary            sut,
            IGattDeviceServiceWrapper         service,
            IGattCharacteristicsResultWrapper result)
        {
            sut[service] = result;

            sut.Count
               .Should()
               .Be(1);
        }

        [AutoDataTestMethod]
        public void Clear_ForInvoked_DisposesService1(
            GattServicesDictionary            sut,
            IGattDeviceServiceWrapper         service1,
            IGattCharacteristicsResultWrapper result1,
            IGattDeviceServiceWrapper         service2,
            IGattCharacteristicsResultWrapper result2)
        {
            sut[service1] = result1;
            sut[service2] = result2;

            sut.Clear();

            service1.Received()
                    .Dispose();
        }

        [AutoDataTestMethod]
        public void Clear_ForInvoked_DisposesService2(
            GattServicesDictionary            sut,
            IGattDeviceServiceWrapper         service1,
            IGattCharacteristicsResultWrapper result1,
            IGattDeviceServiceWrapper         service2,
            IGattCharacteristicsResultWrapper result2)
        {
            sut[service1] = result1;
            sut[service2] = result2;

            sut.Clear();

            sut[service1] = result1;
            sut[service2] = result2;
        }

        [AutoDataTestMethod]
        public void Clear_ForInvoked_SetsCountToZero(
            GattServicesDictionary            sut,
            IGattDeviceServiceWrapper         service,
            IGattCharacteristicsResultWrapper result)
        {
            sut[service] = result;

            sut.Clear();

            sut.Count
               .Should()
               .Be(0);
        }

        [AutoDataTestMethod]
        public void Dispose_ForInvoked_DisposesService1(
            GattServicesDictionary            sut,
            IGattDeviceServiceWrapper         service1,
            IGattCharacteristicsResultWrapper result1,
            IGattDeviceServiceWrapper         service2,
            IGattCharacteristicsResultWrapper result2)
        {
            sut[service1] = result1;
            sut[service2] = result2;

            sut.Dispose();

            service1.Received()
                    .Dispose();
        }

        [AutoDataTestMethod]
        public void Dispose_ForInvoked_DisposesService2(
            GattServicesDictionary            sut,
            IGattDeviceServiceWrapper         service1,
            IGattCharacteristicsResultWrapper result1,
            IGattDeviceServiceWrapper         service2,
            IGattCharacteristicsResultWrapper result2)
        {
            sut[service1] = result1;
            sut[service2] = result2;

            sut.Dispose();

            service2.Received()
                    .Dispose();
        }


        [AutoDataTestMethod]
        public void ReadOnlyDictionary_ForInvoked_ContainsService1(
            GattServicesDictionary            sut,
            IGattDeviceServiceWrapper         service,
            IGattCharacteristicsResultWrapper result)
        {
            sut[service] = result;

            sut.ReadOnlyDictionary[service]
               .Should()
               .Be(result);
        }
    }
}