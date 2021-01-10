using System ;
using System.Collections.Generic ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public sealed class ReferenceOutput
        : UnknownBase , IReferenceOutput
    {
        public Guid                 GattServiceUuid { get ; } = Guid.Empty ;
        public IEnumerable < byte > RawHeightSpeed  { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawTwo          { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawThree        { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawFour         { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawFive         { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawSix          { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawSeven        { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawEight        { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawMask         { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawDetectMask   { get ; } = RawArrayEmpty ;

        public IObservable < RawValueChangedDetails > HeightSpeedChanged =>
            throw new NotInitializeException ( Message ) ;

        internal const string Message = "Can't use a anknown Instance" ;
    }
}