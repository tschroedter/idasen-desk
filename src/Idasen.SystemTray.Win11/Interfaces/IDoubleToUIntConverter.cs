namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDoubleToUIntConverter
{
    uint ConvertToUInt ( double value ,
                         uint   defaultValue ) ;
}