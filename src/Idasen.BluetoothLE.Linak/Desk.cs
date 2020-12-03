using System ;
using System.Collections.Generic ;
using System.Reactive.Subjects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    public class Desk
        : IDesk
    {
        public Desk (
            [ NotNull ] ILogger        logger ,
            [ NotNull ] IDeskConnector connector )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( connector ,
                                    nameof ( connector ) ) ;

            _logger    = logger ;
            _connector = connector ;
        }

        /// <inheritdoc />
        public ulong BluetoothAddress => _connector.BluetoothAddress;

        /// <inheritdoc />
        public string BluetoothAddressType => _connector.BluetoothAddressType;

        /// <inheritdoc />
        public void Connect ( )
        {
            _logger.Debug ( "Connecting to desk..." ) ;

            _connector.Connect ( ) ;
        }

        /// <inheritdoc />
        public ISubject < IEnumerable < byte > > DeviceNameChanged => _connector.DeviceNameChanged ;

        /// <inheritdoc />
        public IObservable < uint > HeightChanged => _connector.HeightChanged ;

        /// <inheritdoc />
        public IObservable < int > SpeedChanged => _connector.SpeedChanged ;

        /// <inheritdoc />
        public IObservable < HeightSpeedDetails > HeightAndSpeedChanged =>
            _connector.HeightAndSpeedChanged ; // todo use only this

        /// <inheritdoc />
        public IObservable < uint > FinishedChanged => _connector.FinishedChanged ;

        /// <inheritdoc />
        public IObservable < bool > RefreshedChanged => _connector.RefreshedChanged ;

        /// <inheritdoc />
        public string Name => _connector.DeviceName ;

        /// <inheritdoc />
        public void MoveTo ( uint targetHeight )
        {
            _logger.Debug ( $"Move desk to target height {targetHeight}" ) ;

            _connector.MoveTo ( targetHeight ) ;
        }

        /// <inheritdoc />
        public void MoveUp ( )
        {
            _logger.Debug ( "Move desk up" ) ;

            _connector.MoveUp ( ) ;
        }

        /// <inheritdoc />
        public void MoveDown ( )
        {
            _logger.Debug ( "Move desk down" ) ;

            _connector.MoveDown ( ) ;
        }

        /// <inheritdoc />
        public void MoveStop ( )
        {
            _logger.Debug ( "Stop moving desk" ) ;

            _connector.MoveStop ( ) ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _connector.Dispose ( ) ;
        }

        private readonly IDeskConnector _connector ;
        private readonly ILogger        _logger ;
    }
}