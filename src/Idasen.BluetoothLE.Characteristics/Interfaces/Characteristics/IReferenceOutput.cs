using System ;
using System.Collections.Generic ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;

// ReSharper disable UnusedMemberInSuper.Global

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface IReferenceOutput
        : ICharacteristicBase
    {
        delegate IReferenceOutput Factory ( IDevice device ) ;

        Guid                                   GattServiceUuid    { get ; }
        IEnumerable < byte >                   RawHeightSpeed     { get ; }
        IEnumerable < byte >                   RawTwo             { get ; }
        IEnumerable < byte >                   RawThree           { get ; }
        IEnumerable < byte >                   RawFour            { get ; }
        IEnumerable < byte >                   RawFive            { get ; }
        IEnumerable < byte >                   RawSix             { get ; }
        IEnumerable < byte >                   RawSeven           { get ; }
        IEnumerable < byte >                   RawEight           { get ; }
        IEnumerable < byte >                   RawMask            { get ; }
        IEnumerable < byte >                   RawDetectMask      { get ; }
        IObservable < RawValueChangedDetails > HeightSpeedChanged { get ; }
    }
}