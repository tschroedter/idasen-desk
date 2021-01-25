using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Threading.Tasks ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeskHeightAndSpeed
        : IDeskHeightAndSpeed
    {
        public DeskHeightAndSpeed ( [ NotNull ] ILogger                            logger ,
                                    [ NotNull ] IScheduler                         scheduler ,
                                    [ NotNull ] IReferenceOutput                   referenceOutput ,
                                    [ NotNull ] IRawValueToHeightAndSpeedConverter converter ,
                                    [ NotNull ] ISubject < uint >                  subjectHeight ,
                                    [ NotNull ] ISubject < int >                   subjectSpeed ,
                                    [ NotNull ] ISubject < HeightSpeedDetails >    subjectHeightAndSpeed )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( referenceOutput ,
                                    nameof ( referenceOutput ) ) ;
            Guard.ArgumentNotNull ( converter ,
                                    nameof ( converter ) ) ;
            Guard.ArgumentNotNull ( subjectHeight ,
                                    nameof ( subjectHeight ) ) ;
            Guard.ArgumentNotNull ( subjectSpeed ,
                                    nameof ( subjectSpeed ) ) ;
            Guard.ArgumentNotNull ( subjectHeightAndSpeed ,
                                    nameof ( subjectHeightAndSpeed ) ) ;

            _logger                = logger ;
            _scheduler             = scheduler ;
            _referenceOutput       = referenceOutput ;
            _converter             = converter ;
            _subjectHeight         = subjectHeight ;
            _subjectSpeed          = subjectSpeed ;
            _subjectHeightAndSpeed = subjectHeightAndSpeed ;
        }

        public IObservable < uint > HeightChanged => _subjectHeight ;

        public IObservable < int > SpeedChanged => _subjectSpeed ;

        public IObservable < HeightSpeedDetails > HeightAndSpeedChanged => _subjectHeightAndSpeed ;

        public uint Height { get ; private set ; }

        public int Speed { get ; private set ; }

        public async Task Refresh ( )
        {
            await _referenceOutput.Refresh ( ) ;

            Initialize ( ) ;
        }

        public IDeskHeightAndSpeed Initialize ( )
        {
            _subscriber?.Dispose ( ) ;

            _subscriber = _referenceOutput.HeightSpeedChanged
                                          .ObserveOn ( _scheduler )
                                          .Subscribe ( OnHeightSpeedChanged ) ;

            if ( _subscriber is UnknownBase )
                _logger.Warning ( $"{nameof ( _referenceOutput )} is set to Unknown" ) ;

            if ( ! _converter.TryConvert ( _referenceOutput.RawHeightSpeed ,
                                           out var height ,
                                           out var speed ) )
                return this ;

            Height = height ;
            Speed  = speed ;

            return this ;
        }

        public void Dispose ( )
        {
            _referenceOutput?.Dispose ( ) ;
            _subscriber?.Dispose ( ) ;
        }

        public delegate IDeskHeightAndSpeed Factory ( IReferenceOutput referenceOutput ) ;

        private void OnHeightSpeedChanged ( RawValueChangedDetails details )
        {
            if ( details == null )
                return ;

            if ( ! _converter.TryConvert ( details.Value ,
                                           out var height ,
                                           out var speed ) )
                return ;

            Height = height ;
            Speed  = speed ;

            _subjectHeight.OnNext ( Height ) ;
            _subjectSpeed.OnNext ( Speed ) ;

            var value = new HeightSpeedDetails ( details.Timestamp ,
                                                 Height ,
                                                 Speed ) ;

            _subjectHeightAndSpeed.OnNext ( value ) ;

            _logger.Debug ( $"Height = {Height} (10ths of a millimeter), Speed = {Speed} (100/RPM)" ) ;
        }

        private readonly IRawValueToHeightAndSpeedConverter _converter ;

        private readonly ILogger                         _logger ;
        private readonly IReferenceOutput                _referenceOutput ;
        private readonly IScheduler                      _scheduler ;
        private readonly ISubject < uint >               _subjectHeight ;
        private readonly ISubject < HeightSpeedDetails > _subjectHeightAndSpeed ;
        private readonly ISubject < int >                _subjectSpeed ;

        private IDisposable _subscriber ;
    }
}