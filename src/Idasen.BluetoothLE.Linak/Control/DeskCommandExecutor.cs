using System.Threading.Tasks ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Control
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeskCommandExecutor
        : IDeskCommandExecutor
    {
        public DeskCommandExecutor ( [ NotNull ] ILogger               logger ,
                                     [ NotNull ] IErrorManager         errorManager ,
                                     [ NotNull ] IDeskCommandsProvider provider ,
                                     [ NotNull ] IControl              control )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( provider ,
                                    nameof ( provider ) ) ;
            Guard.ArgumentNotNull ( control ,
                                    nameof ( control ) ) ;
            Guard.ArgumentNotNull ( errorManager ,
                                    nameof ( errorManager ) ) ;

            _logger       = logger ;
            _errorManager = errorManager ;
            _provider     = provider ;
            _control      = control ;
        }

        public async Task < bool > Up ( )
        {
            return await Execute ( DeskCommands.MoveUp ) ;
        }

        public async Task < bool > Down ( )
        {
            return await Execute ( DeskCommands.MoveDown ) ;
        }

        public async Task < bool > Stop ( )
        {
            return await Execute ( DeskCommands.MoveStop ) ;
        }

        public delegate IDeskCommandExecutor Factory ( IControl control ) ;

        private async Task < bool > Execute ( DeskCommands deskCommand )
        {
            if ( ! _provider.TryGetValue ( deskCommand ,
                                           out var bytes ) )
            {
                _logger.Error ( $"Failed for unknown command '{deskCommand}'" ) ;

                return false ;
            }

            var result = await _control.TryWriteRawControl2 ( bytes ) ;

            if ( ! result )
                ExecutionFailed ( deskCommand ) ; // to do testing

            return result ;
        }

        private void ExecutionFailed ( DeskCommands deskCommand )
        {
            var message = $"Failed for '{deskCommand}' command. " +
                          Constants.CheckAndEnableBluetooth ;

            _logger.Error ( message ) ;

            _errorManager.PublishForMessage ( Constants.CheckAndEnableBluetooth ) ;
        }

        private readonly IControl      _control ;
        private readonly IErrorManager _errorManager ;

        private readonly ILogger               _logger ;
        private readonly IDeskCommandsProvider _provider ;
    }
}