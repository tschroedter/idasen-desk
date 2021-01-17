using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public class ReferenceOutput
        : CharacteristicBase ,
          IReferenceOutput
    {
        public ReferenceOutput (
            ILogger                                         logger ,
            IScheduler                                      scheduler ,
            IDevice                                         device ,
            IGattCharacteristicsProviderFactory             providerFactory ,
            IRawValueReader                                 rawValueReader ,
            IRawValueWriter                                 rawValueWriter ,
            ICharacteristicBaseToStringConverter            toStringConverter ,
            IDescriptionToUuid                              descriptionToUuid ,
            [ NotNull ] ISubject < RawValueChangedDetails > subjectHeightSpeed )
            : base ( logger ,
                     scheduler ,
                     device ,
                     providerFactory ,
                     rawValueReader ,
                     rawValueWriter ,
                     toStringConverter ,
                     descriptionToUuid )
        {
            Guard.ArgumentNotNull ( subjectHeightSpeed ,
                                    nameof ( subjectHeightSpeed ) ) ;

            _subjectHeightSpeed = subjectHeightSpeed ;
        }

        public IObservable < RawValueChangedDetails > HeightSpeedChanged => _subjectHeightSpeed ;

        public override Guid GattServiceUuid { get ; } = Guid.Parse ( "99FA0020-338A-1024-8A49-009C0215F78A" ) ;

        public IEnumerable < byte > RawHeightSpeed => GetValueOrEmpty ( HeightSpeed ) ;
        public IEnumerable < byte > RawTwo         => GetValueOrEmpty ( Two ) ;
        public IEnumerable < byte > RawThree       => GetValueOrEmpty ( Three ) ;
        public IEnumerable < byte > RawFour        => GetValueOrEmpty ( Four ) ;
        public IEnumerable < byte > RawFive        => GetValueOrEmpty ( Five ) ;
        public IEnumerable < byte > RawSix         => GetValueOrEmpty ( Six ) ;
        public IEnumerable < byte > RawSeven       => GetValueOrEmpty ( Seven ) ;
        public IEnumerable < byte > RawEight       => GetValueOrEmpty ( Eight ) ;
        public IEnumerable < byte > RawMask        => GetValueOrEmpty ( Mask ) ;
        public IEnumerable < byte > RawDetectMask  => GetValueOrEmpty ( DetectMask ) ;

        public override T Initialize < T > ( )
        {
            base.Initialize < T > ( ) ;

            return this as T ;
        }

        public override async Task Refresh ( )
        {
            await base.Refresh ( ) ;

            if ( ! Characteristics.Characteristics.TryGetValue ( HeightSpeed ,
                                                                 out var heightAndSpeed ) ||
                 heightAndSpeed == null )
            {
                Logger.Error ( "Failed to find characteristic for Height and Speed " +
                               $"with UUID '{HeightSpeed}'" ) ;

                return ;
            }

            _subscriber?.Dispose ( ) ; // Attention: this is to hard to test

            _subscriber = heightAndSpeed.ValueChanged
                                        .SubscribeOn ( Scheduler )
                                        .Subscribe ( OnValueChanged ) ;

            var rawValue = GetValueOrEmpty ( HeightSpeed ).ToArray ( ) ;

            var details = new RawValueChangedDetails ( HeightSpeed ,
                                                       rawValue ,
                                                       DateTimeOffset.Now ,
                                                       GattServiceUuid ) ;

            _subjectHeightSpeed.OnNext ( details ) ;
        }

        public delegate IReferenceOutput Factory ( IDevice device ) ;

        internal const string HeightSpeed = "Height Speed" ;
        internal const string Two         = "TWO" ;
        internal const string Three       = "THREE" ;
        internal const string Four        = "FOUR" ;
        internal const string Five        = "FIVE" ;
        internal const string Six         = "SIX" ;
        internal const string Seven       = "SEVEN" ;
        internal const string Eight       = "EIGHT" ;
        internal const string Mask        = "MASK" ;
        internal const string DetectMask  = "DETECT MASK" ;

        protected override void Dispose ( bool disposing )
        {
            if ( _disposed ) return ;

            if ( disposing ) _subscriber?.Dispose ( ) ;

            _disposed = true ;

            base.Dispose ( disposing ) ;
        }

        protected override T WithMapping < T > ( ) where T : class
        {
            DescriptionToUuid [ HeightSpeed ] = Guid.Parse ( "99FA0021-338A-1024-8A49-009C0215F78A" ) ;
            DescriptionToUuid [ Mask ]        = Guid.Parse ( "99FA0029-338A-1024-8A49-009C0215F78A" ) ;
            DescriptionToUuid [ DetectMask ]  = Guid.Parse ( "99FA002A-338A-1024-8A49-009C0215F78A" ) ;

            return this as T ;
        }

        private void OnValueChanged ( GattCharacteristicValueChangedDetails details )
        {
            if ( details == null )
            {
                Logger.Error ( $"{typeof ( GattCharacteristicValueChangedDetails )} is null" ) ;

                return ;
            }

            Logger.Debug ( $"Value = {details.Value.ToHex ( )}, " +
                           $"Timestamp = {details.Timestamp}, "   +
                           $"Uuid = {details.Uuid}" ) ;

            if ( details.Value.Count ( ) != 4 )
            {
                Logger.Error ( $"Failed, expected 4 bytes but received {details.Value.Count ( )}" ) ;
                return ;
            }

            var valueChanged = new RawValueChangedDetails ( HeightSpeed ,
                                                            details.Value ,
                                                            details.Timestamp ,
                                                            details.Uuid ) ;

            _subjectHeightSpeed.OnNext ( valueChanged ) ;
        }

        private readonly ISubject < RawValueChangedDetails > _subjectHeightSpeed ;

        private bool _disposed ;

        private IDisposable _subscriber ;
    }
}