namespace Idasen.SystemTray.Win11.Interfaces
{
    internal interface IDoubleToUIntConverter
    {
        uint ConvertToUInt ( double value ,
                             uint   defaultValue ) ;
    }
}