using System ;
using System.Reactive.Subjects ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.DevicesDiscovery
{
    /// <inheritdoc cref="IWatcher" />
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class Watcher
        : IWatcher
    {
        public Watcher ( [ NotNull ] IWrapper              wrapper ,
                         [ NotNull ] ISubject < DateTime > started )
        {
            Guard.ArgumentNotNull ( wrapper ,
                                    nameof ( wrapper ) ) ;
            Guard.ArgumentNotNull ( started ,
                                    nameof ( started ) ) ;

            _wrapper         = wrapper ;
            _startedWatching = started ;
        }

        /// <inheritdoc />
        public bool IsListening => _wrapper.Status == Status.Started ;

        /// <inheritdoc />
        public IObservable < DateTime > Started => _startedWatching ;

        /// <inheritdoc />
        public IObservable < DateTime > Stopped => _wrapper.Stopped ;

        /// <inheritdoc />
        public IObservable < IDevice > Received => _wrapper.Received ;

        /// <inheritdoc />
        public void Start ( )
        {
            if ( IsListening )
                return ;

            _wrapper.Start ( ) ;

            _startedWatching.OnNext ( DateTime.Now ) ;
        }

        /// <inheritdoc />
        public void Stop ( )
        {
            if ( ! IsListening )
                return ;

            _wrapper.Stop ( ) ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _wrapper.Dispose ( ) ;
        }

        private readonly ISubject < DateTime > _startedWatching ;
        private readonly IWrapper              _wrapper ;
    }
}