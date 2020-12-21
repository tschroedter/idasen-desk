using System.Diagnostics.CodeAnalysis ;
using Autofac ;
using Idasen.BluetoothLE.Linak ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Settings ;

namespace Idasen.SystemTray
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public class SystemTrayModule
        : Module
    {
        protected override void Load ( ContainerBuilder builder )
        {
            builder.RegisterModule < BluetoothLELinakModule > ( ) ;

            builder.RegisterType <SettingsManager> ( )
                   .As <ISettingsManager> ( ) ;
        }
    }
}