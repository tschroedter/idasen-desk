using System ;
using System.Reactive.Subjects ;
using System.Runtime.CompilerServices ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class ErrorManager // todo testing, move to more general project
        : IErrorManager
    {
        public ErrorManager (
            [ NotNull ] ILogger                    logger ,
            [ NotNull ] ISubject < IErrorDetails > subject )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( subject ,
                                    nameof ( subject ) ) ;
            _logger  = logger ;
            _subject = subject ;
        }

        /// <inheritdoc />
        public void Publish ( IErrorDetails details )
        {
            Guard.ArgumentNotNull ( details ,
                                    nameof ( details ) ) ;

            _logger.Debug ( $"Received {details}" ) ;

            _subject.OnNext ( details ) ;
        }

        /// <inheritdoc />
        public void PublishForMessage ( string                      message ,
                                        [ CallerMemberName ] string caller = "" )
        {
            Guard.ArgumentNotNull ( message ,
                                    nameof ( message ) ) ;

            _logger.Debug ( $"Received {message}" ) ;

            _subject.OnNext ( new ErrorDetails ( message ,
                                                 caller ) ) ;
        }

        public           IObservable < IErrorDetails > ErrorChanged => _subject ;
        private readonly ILogger                       _logger ;
        private readonly ISubject < IErrorDetails >    _subject ;
    }
}