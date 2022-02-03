using System ;
using System.Collections.Generic ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class Desk
        : IDesk
    {
        public Desk (
            [ NotNull ] IDeskConnector connector )
        {
            Guard.ArgumentNotNull ( connector ,
                                    nameof ( connector ) ) ;

            _connector = connector ;
        }

        /// <inheritdoc />
        public ulong BluetoothAddress => _connector.BluetoothAddress ;

        /// <inheritdoc />
        public string BluetoothAddressType => _connector.BluetoothAddressType ;

        /// <inheritdoc />
        public void Connect ( )
        {
            _connector.Connect ( ) ;
        }

        /// <inheritdoc />
        public IObservable < IEnumerable < byte > > DeviceNameChanged => _connector.DeviceNameChanged ;

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
            _connector.MoveTo ( targetHeight ) ;
        }

        /// <inheritdoc />
        public void MoveUp ( )
        {
            _connector.MoveUp ( ) ;
        }

        /// <inheritdoc />
        public void MoveDown ( )
        {
            _connector.MoveDown ( ) ;
        }

        /// <inheritdoc />
        public void MoveStop ( )
        {
            _connector.MoveStop ( ) ;
        }

        /// <inheritdoc />
        public void MoveLock ( )
        {
            _connector.MoveLock ( ) ;
        }

        /// <inheritdoc />
        public void MoveUnlock()
        {
            _connector.MoveUnlock ( ) ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _connector.Dispose ( ) ;
        }

        /// <inheritdoc />
        public string DeviceName => _connector.DeviceName ;

        private readonly IDeskConnector _connector ;
    }
}