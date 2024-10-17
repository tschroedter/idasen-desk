namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IStringToUIntConverter
{
    ulong ConvertStringToUlongOrDefault ( string text ,
                                          ulong  defaultValue ) ;
}