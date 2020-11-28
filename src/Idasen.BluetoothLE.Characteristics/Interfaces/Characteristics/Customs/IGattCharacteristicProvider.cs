using System ;
using System.Collections.Generic ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

// ReSharper disable UnusedMemberInSuper.Global

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs
{
    public interface IGattCharacteristicProvider
    {
        IReadOnlyDictionary < string , IGattCharacteristicWrapper >   Characteristics            { get ; }
        IReadOnlyCollection < string >                                UnavailableCharacteristics { get ; }
        IReadOnlyDictionary < string , GattCharacteristicProperties > Properties                 { get ; }

        void Refresh (
            [ NotNull ] IReadOnlyDictionary < string , Guid > customCharacteristic ) ;
    }
}