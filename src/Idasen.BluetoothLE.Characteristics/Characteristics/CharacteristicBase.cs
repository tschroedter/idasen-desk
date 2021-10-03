using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reactive.Concurrency ;
using System.Runtime.CompilerServices ;
using System.Runtime.InteropServices.WindowsRuntime ;
using System.Threading.Tasks ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using JetBrains.Annotations ;
using Serilog ;

[ assembly : InternalsVisibleTo ( "Idasen.BluetoothLE.Characteristics.Tests" ) ]

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public abstract class CharacteristicBase
        : ICharacteristicBase
    {
        protected CharacteristicBase (
            [ NotNull ] ILogger                              logger ,
            [ NotNull ] IScheduler                           scheduler ,
            [ NotNull ] IDevice                              device ,
            [ NotNull ] IGattCharacteristicsProviderFactory  providerFactory ,
            [ NotNull ] IRawValueReader                      rawValueReader ,
            [ NotNull ] IRawValueWriter                      rawValueWriter ,
            [ NotNull ] ICharacteristicBaseToStringConverter toStringConverter ,
            [ NotNull ] IDescriptionToUuid                   descriptionToUuid )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( device ,
                                    nameof ( device ) ) ;
            Guard.ArgumentNotNull ( providerFactory ,
                                    nameof ( providerFactory ) ) ;
            Guard.ArgumentNotNull ( rawValueReader ,
                                    nameof ( rawValueReader ) ) ;
            Guard.ArgumentNotNull ( rawValueWriter ,
                                    nameof ( rawValueWriter ) ) ;
            Guard.ArgumentNotNull ( toStringConverter ,
                                    nameof ( toStringConverter ) ) ;
            Guard.ArgumentNotNull ( descriptionToUuid ,
                                    nameof ( descriptionToUuid ) ) ;

            Device             = device ;
            Logger             = logger ;
            Scheduler          = scheduler ;
            ProviderFactory    = providerFactory ;
            RawValueReader     = rawValueReader ;
            RawValueWriter     = rawValueWriter ;
            _toStringConverter = toStringConverter ;
            DescriptionToUuid  = descriptionToUuid ;
        }

        public virtual T Initialize < T > ( )
            where T : class
        {
            Guard.ArgumentNotNull ( GattServiceUuid ,
                                    nameof ( GattServiceUuid ) ) ;

            var (service , characteristicsResultWrapper) = Device.GattServices
                                                                 .FirstOrDefault ( x => x.Key.Uuid ==
                                                                                        GattServiceUuid ) ;

            if ( service == null )
            {
                foreach ( var service1 in Device.GattServices )
                {
                    Logger.Information ( $"Service: DeviceId = {service1.Key.DeviceId}, Uuid = {service1.Key.Uuid}" );

                    foreach ( var characteristic in service1.Value.Characteristics)
                    {
                        Logger.Information($"Characteristic: {characteristic.ServiceUuid} {characteristic.Uuid} {characteristic.UserDescription}");
                    }
                }
                throw new ArgumentException ( "Failed, can't find GattDeviceService for " +
                                              $"UUID {GattServiceUuid}" ,
                                              nameof ( GattServiceUuid ) ) ;
            }

            Logger.Information ( $"Found GattDeviceService with UUID {GattServiceUuid}" ) ;

            Characteristics = ProviderFactory.Create ( characteristicsResultWrapper ) ;

            WithMapping < T > ( ) ;

            return this as T ;
        }

        public virtual async Task Refresh ( )
        {
            Characteristics.Refresh ( DescriptionToUuid.ReadOnlyDictionary ) ;

            var keys = Characteristics.Characteristics.Keys.ToArray ( ) ;

            foreach ( var key in keys )
            {
                if ( ! Characteristics.Characteristics.TryGetValue ( key ,
                                                                     out var characteristic ) )
                {
                    Logger.Warning ( $"Failed to get value for key '{key}'" ) ;

                    continue ;
                }

                if ( characteristic == null )
                {
                    Logger.Warning ( $"Failed, characteristic for key '{key}' is null" ) ;

                    continue ;
                }

                Logger.Debug ( $"Reading raw value for {key} " +
                               $"and and characteristic {characteristic.Uuid}" ) ;

                (bool success , byte [ ] value) result =
                    await RawValueReader.TryReadValueAsync ( characteristic ) ;

                RawValues [ key ] = result.success
                                        ? result.value
                                        : RawArrayEmpty ;
            }
        }

        public void Dispose ( )
        {
            Dispose ( true ) ;
        }

        internal static readonly IEnumerable < byte > RawArrayEmpty = Enumerable.Empty < byte > ( )
                                                                                .ToArray ( ) ;

        public abstract Guid GattServiceUuid { get ; }

        internal IDescriptionToUuid DescriptionToUuid { get ; }

        protected abstract T WithMapping < T > ( )
            where T : class ;

        protected async Task < bool > TryWriteValueAsync ( string               key ,
                                                           IEnumerable < byte > bytes )
        {
            try
            {
                return await DoTryWriteValueAsync ( key ,
                                                    bytes ) ;
            }
            catch ( Exception e )
            {
                const string message = "Failed to write value async!" ;

                if ( e.IsBluetoothDisabledException ( ) )
                    e.LogBluetoothStatusException ( Logger ,
                                                    message ) ;
                else
                    Logger.Error ( e ,
                                   message ) ;

                return false ;
            }
        }

        private async Task < bool > DoTryWriteValueAsync ( string               key ,
                                                           IEnumerable < byte > bytes )
        {
            if ( ! Characteristics.Characteristics.TryGetValue ( key ,
                                                                 out var characteristic ) )
            {
                Logger.Error ( $"Unknown characteristic with key '{key}'" ) ;

                return false ;
            }

            if ( characteristic != null )
                return await RawValueWriter.TryWriteValueAsync ( characteristic ,
                                                                 bytes.ToArray ( )
                                                                      .AsBuffer ( ) ) ;

            Logger.Error ( $"Characteristic for key '{key}' is null" ) ;

            return false ;
        }

        [ NotNull ]
        protected IEnumerable < byte > GetValueOrEmpty ( string key )
        {
            return RawValues.TryGetValue ( key ,
                                           out var values )
                       ? values
                       : RawArrayEmpty ;
        }

        public override string ToString ( )
        {
            return _toStringConverter.ToString ( this ) ;
        }

        protected virtual void Dispose ( bool disposing )
        {
            if ( _disposed ) return ;

            _disposed = true ;
        }

        private readonly ICharacteristicBaseToStringConverter _toStringConverter ;

        protected readonly IDevice                             Device ;
        protected readonly ILogger                             Logger ;
        protected readonly IGattCharacteristicsProviderFactory ProviderFactory ;
        protected readonly IRawValueReader                     RawValueReader ;

        internal readonly Dictionary < string , IEnumerable < byte > >
            RawValues = new Dictionary < string , IEnumerable < byte > > ( ) ;

        protected readonly IRawValueWriter RawValueWriter ;

        protected readonly IScheduler Scheduler ;

        private bool _disposed ;

        internal IGattCharacteristicProvider Characteristics ;
    }
}