﻿using System;
using System.Threading.Tasks;

namespace Idasen.BluetoothLE.Linak.Control
{
    public interface IDeskMover
        : IDisposable
    {
        uint              Height       { get; }
        int               Speed        { get; }
        uint              TargetHeight { get; set; }
        IObservable<uint> Finished     { get; }
        Task<bool>        Up();
        Task<bool>        Down();
        Task<bool>        Stop();
        void              Start();
        void              Initialize ( ) ;
    }
}