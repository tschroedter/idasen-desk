using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public class GenericAccess
        : UnknownBase, IGenericAccess
    {
        public GenericAccess()
        {
            GattServiceUuid = Guid.Empty;
        }

        public IEnumerable<byte> RawResolution { get; } = RawArrayEmpty;
        public IEnumerable<byte> RawParameters { get; } = RawArrayEmpty;
        public IEnumerable<byte> RawAppearance { get; } = RawArrayEmpty;
        public IEnumerable<byte> RawDeviceName { get; } = RawArrayEmpty;

        public ISubject<IEnumerable<byte>> AppearanceChanged =>
            throw new NotInitializeException("Call Initialize() first!");

        public ISubject<IEnumerable<byte>> ParametersChanged =>
            throw new NotInitializeException("Call Initialize() first!");

        public ISubject<IEnumerable<byte>> ResolutionChanged =>
            throw new NotInitializeException("Call Initialize() first!");

        public ISubject<IEnumerable<byte>> DeviceNameChanged =>
            throw new NotInitializeException("Call Initialize() first!");

        public Guid GattServiceUuid { get; }
    }
}