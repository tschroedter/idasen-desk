namespace Idasen.SystemTray.Interfaces
{
    internal interface IDoubleToUIntConverter
    {
        bool TryConvertToUInt ( double   value ,
                                out uint uintValue ) ;

        uint TryConvertToUInt(double value,
                              uint   defaultValue) ;
    }
}