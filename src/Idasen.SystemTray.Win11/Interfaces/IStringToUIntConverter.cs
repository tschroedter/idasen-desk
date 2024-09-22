namespace Idasen.SystemTray.Win11.Interfaces
{
    internal interface IStringToUIntConverter
    {
        ulong ConvertStringToUlongOrDefault(string text,
                             ulong  defaultValue);
    }
}