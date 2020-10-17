using System;

namespace Idasen.BluetoothLE.Linak
{
    public class HeightSpeedDetails
    {
        public DateTimeOffset Timestamp { get; }
        public uint           Height    { get; }
        public int            Speed     { get; }

        public HeightSpeedDetails(DateTimeOffset timestamp,
                                  uint           height,
                                  int            speed)
        {
            Timestamp = timestamp;
            Height    = height;
            Speed     = speed;
        }

        public override string ToString()
        {
            return $"Timestamp = {Timestamp}, " +
                   $"Height = {Height}, "       +
                   $"Speed = {Speed}";
        }
    }
}