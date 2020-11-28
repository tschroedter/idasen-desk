using System ;
using System.Collections.Generic ;
using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Subjects ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ ExcludeFromCodeCoverage ]
    public class GattCharacteristicWrapper
        : IGattCharacteristicWrapper
    {
        public GattCharacteristicWrapper (
            [ JetBrains.Annotations.NotNull ] ILogger                                            logger ,
            [ JetBrains.Annotations.NotNull ] ISubject < GattCharacteristicValueChangedDetails > valueChanged ,
            [ JetBrains.Annotations.NotNull ] GattCharacteristic                                 characteristic ,
            [ JetBrains.Annotations.NotNull ] IGattCharacteristicValueChangedObservables         observables ,
            [ JetBrains.Annotations.NotNull ] IGattWriteResultWrapperFactory                     factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( valueChanged ,
                                    nameof ( valueChanged ) ) ;
            Guard.ArgumentNotNull ( characteristic ,
                                    nameof ( characteristic ) ) ;
            Guard.ArgumentNotNull ( observables ,
                                    nameof ( observables ) ) ;

            _logger         = logger ;
            _characteristic = characteristic ;
            _observables    = observables ;
            _factory        = factory ;
        }

        /// <inheritdoc />
        public async Task < IGattCharacteristicWrapper > Initialize ( )
        {
            _logger.Information ( "Initializing GattCharacteristic with UUID " +
                                  $"{_characteristic.Uuid}" ) ;

            await _observables.Initialise ( _characteristic ) ;

            return this ;
        }

        /// <inheritdoc />
        public IObservable < GattCharacteristicValueChangedDetails > ValueChanged => _observables.ValueChanged ;

        /// <inheritdoc />
        public Guid Uuid => _characteristic.Uuid ;

        /// <inheritdoc />
        public GattCharacteristicProperties CharacteristicProperties => _characteristic.CharacteristicProperties ;

        public IReadOnlyList < GattPresentationFormat > PresentationFormats => _characteristic.PresentationFormats ;
        public Guid ServiceUuid => _characteristic.Service.Uuid ; // maybe inject IGattDeviceServiceWrapper
        public string UserDescription => _characteristic.UserDescription ;
        public GattProtectionLevel ProtectionLevel => _characteristic.ProtectionLevel ;
        public ushort AttributeHandle => _characteristic.AttributeHandle ;

        /// <inheritdoc />
        public async Task < IGattWriteResult > WriteValueWithResultAsync ( IBuffer buffer )
        {
            var result = await _characteristic.WriteValueWithResultAsync ( buffer ) ;

            return new GattWriteResultWrapper ( result ) ; // todo use excisting factory
        }

        /// <inheritdoc />
        public async Task < GattCommunicationStatus > WriteValueAsync ( IBuffer buffer )
        {
            return await _characteristic.WriteValueAsync ( buffer ) ;
        }

        /// <inheritdoc />
        public Task < IGattReadResult > ReadValueAsync ( )
        {
            var result = _characteristic.ReadValueAsync ( )
                                        .AsTask ( ) ;

            var wrapper = new GattReadResultWrapper ( result.Result ) ; // todo use existing factory

            return Task.FromResult ( ( IGattReadResult ) wrapper ) ;
        }

        public void Dispose ( )
        {
            _observables.Dispose ( ) ;
        }

        public delegate IGattCharacteristicWrapper Factory ( GattCharacteristic characteristic ) ;

        private readonly GattCharacteristic             _characteristic ; // todo use it
        private readonly IGattWriteResultWrapperFactory _factory ;

        private readonly ILogger                                    _logger ;
        private readonly IGattCharacteristicValueChangedObservables _observables ;
    }
}