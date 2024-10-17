namespace Idasen.SystemTray.Interfaces
{
    internal interface IStringToUIntConverter
    {
        ulong ConvertStringToUlongOrDefault(string text,
                             ulong  defaultValue);
    }
}