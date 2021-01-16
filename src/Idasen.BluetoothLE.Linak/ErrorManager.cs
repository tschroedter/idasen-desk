using System ;
using System.Reactive.Subjects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
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

        public           IObservable < IErrorDetails > ErrorChanged => _subject ;
        private readonly ILogger                       _logger ;
        private readonly ISubject < IErrorDetails >    _subject ;
    }
}