using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Idasen.BluetoothLE.Characteristics.Common;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers;
using JetBrains.Annotations;
using Serilog;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public class ReferenceOutput
        : CharacteristicBase,
          IReferenceOutput
    {
        public delegate IReferenceOutput Factory(IDevice device);

        internal const string HeightSpeed = "Height Speed";
        internal const string Two         = "TWO";
        internal const string Three       = "THREE";
        internal const string Four        = "FOUR";
        internal const string Five        = "FIVE";
        internal const string Six         = "SIX";
        internal const string Seven       = "SEVEN";
        internal const string Eight       = "EIGHT";
        internal const string Mask        = "MASK";
        internal const string DetectMask  = "DETECT MASK";

        private readonly ISubject<RawValueChangedDetails> _subjectHeightSpeed;

        private IDisposable                 _subscriber;

        public ReferenceOutput (
            ILogger                                         logger ,
            IScheduler                                      scheduler ,
            IDevice                                         device ,
            IGattCharacteristicsProviderFactory             providerFactory ,
            IRawValueReader                                 rawValueReader ,
            IRawValueWriter                                 rawValueWriter ,
            ICharacteristicBaseToStringConverter            toStringConverter ,
            [ NotNull ] ISubject < RawValueChangedDetails > subjectHeightSpeed )
            : base ( logger ,
                     scheduler ,
                     device ,
                     providerFactory ,
                     rawValueReader ,
                     rawValueWriter ,
                     toStringConverter )
        {
            Guard.ArgumentNotNull ( subjectHeightSpeed ,
                                    nameof ( subjectHeightSpeed ) ) ;

            _subjectHeightSpeed = subjectHeightSpeed ;
        }

        public IObservable<RawValueChangedDetails> HeightSpeedChanged => _subjectHeightSpeed;

        public override Guid GattServiceUuid { get; } = Guid.Parse("99FA0020-338A-1024-8A49-009C0215F78A");

        public IEnumerable<byte> RawHeightSpeed => TryGetValueOrEmpty(HeightSpeed);
        public IEnumerable<byte> RawTwo         => TryGetValueOrEmpty(Two);
        public IEnumerable<byte> RawThree       => TryGetValueOrEmpty(Three);
        public IEnumerable<byte> RawFour        => TryGetValueOrEmpty(Four);
        public IEnumerable<byte> RawFive        => TryGetValueOrEmpty(Five);
        public IEnumerable<byte> RawSix         => TryGetValueOrEmpty(Six);
        public IEnumerable<byte> RawSeven       => TryGetValueOrEmpty(Seven);
        public IEnumerable<byte> RawEight       => TryGetValueOrEmpty(Eight);
        public IEnumerable<byte> RawMask        => TryGetValueOrEmpty(Mask);
        public IEnumerable<byte> RawDetectMask  => TryGetValueOrEmpty(DetectMask);

        public override T Initialize<T>()
        {
            base.Initialize<T>();

            return this as T;
        }

        public override async Task Refresh()
        {
            await base.Refresh();

            _subscriber?.Dispose(); // todo this line needs testing

            if( !Characteristics.Characteristics.TryGetValue(HeightSpeed,
                                                             out var heightAndSpeed) ||
                heightAndSpeed == null)
            {
                Logger.Error("Failed to find characteristic for Height and Speed " +
                             $"with UUID '{HeightSpeed}'");

                return;
            }

            _subscriber = heightAndSpeed.ValueChanged
                                        .SubscribeOn(Scheduler)
                                        .Subscribe(OnValueChanged); // todo this line needs testing

            var rawValue = TryGetValueOrEmpty(HeightSpeed).ToArray();

            var valueChanged = new RawValueChangedDetails(HeightSpeed,
                                                          rawValue,
                                                          DateTimeOffset.Now,
                                                          GattServiceUuid);

            _subjectHeightSpeed.OnNext(valueChanged);
        }


        public bool TryConvert(IEnumerable<byte> bytes,
                               out uint          height,
                               out int           speed)
        {
            var array = bytes as byte[] ?? bytes.ToArray();

            try
            {
                var rawHeight = array.Take(2)
                                     .ToArray();

                var rawSpeed = array.Skip(2)
                                    .Take(2)
                                    .ToArray();

                height = 6200u + BitConverter.ToUInt16(rawHeight);
                speed  = BitConverter.ToInt16(rawSpeed);

                return true;
            }
            catch (Exception e)
            {
                Logger.Warning($"Failed to convert raw value '{array.ToHex()}' " +
                                $"to height and speed! ({e.Message})");

                height = 0;
                speed  = 0;

                return false;
            }
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        protected override T WithMapping<T>() where T : class
        {
            DescriptionToUuid[HeightSpeed] = Guid.Parse("99FA0021-338A-1024-8A49-009C0215F78A");
            DescriptionToUuid[Mask]        = Guid.Parse("99FA0029-338A-1024-8A49-009C0215F78A");
            DescriptionToUuid[DetectMask]  = Guid.Parse("99FA002A-338A-1024-8A49-009C0215F78A");

            return this as T;
        }

        private void OnValueChanged(GattCharacteristicValueChangedDetails details)
        {
            Logger.Debug($"Value = {details.Value.ToHex()}, " +
                         $"Timestamp = {details.Timestamp}, " +
                         $"Uuid = {details.Uuid}");

            if (details.Value.Count() != 4)
            {
                Logger.Error($"Failed, expected 4 bytes but received {details.Value.Count()}");
                return;
            }

            var valueChanged = new RawValueChangedDetails(HeightSpeed,
                                                          details.Value,
                                                          details.Timestamp,
                                                          details.Uuid);

            _subjectHeightSpeed.OnNext(valueChanged);
        }
    }
}