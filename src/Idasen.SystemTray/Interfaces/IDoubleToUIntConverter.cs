namespace Idasen.SystemTray.Interfaces
{
    internal interface IDoubleToUIntConverter
    {
        uint ConvertToUInt ( double value ,
                             uint   defaultValue ) ;
    }
}