using System;
using System.Collections.Generic;
using Idasen.BluetoothLE.Characteristics.Common;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public class RawValueChangedDetails
    {
        public RawValueChangedDetails(string            description,
                                      IEnumerable<byte> value,
                                      DateTimeOffset    timestamp,
                                      Guid              uuid)
        {
            Value       = value;
            Timestamp   = timestamp;
            Uuid        = uuid;
            Description = description;
        }

        public string            Description { get; }
        public IEnumerable<byte> Value       { get; }
        public DateTimeOffset    Timestamp   { get; }
        public Guid              Uuid        { get; }

        public override string ToString()
        {
            return $"Value =  {Value.ToHex()}, " +
                   $"Timestamp = {Timestamp}, "  +
                   $"Uuid = {Uuid}";
        }
    }
}