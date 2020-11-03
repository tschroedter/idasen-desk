using System;
using System.Collections.Generic;

namespace Idasen.BluetoothLE.Linak.Control
{
    public interface IRawValueChangedDetailsCollector
        : IDisposable
    {
        IEnumerable<HeightSpeedDetails>  Details { get; }
        IRawValueChangedDetailsCollector Initialize () ;
    }
}