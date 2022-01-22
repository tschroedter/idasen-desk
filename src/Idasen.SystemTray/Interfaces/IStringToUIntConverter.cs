namespace Idasen.SystemTray.Interfaces
{
    internal interface IStringToUIntConverter
    {
        ulong ConvertToULong(string text,
                             ulong  defaultValue);
    }
}