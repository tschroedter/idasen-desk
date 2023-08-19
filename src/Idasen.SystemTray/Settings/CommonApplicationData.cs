using System;
using System.IO;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils;

namespace Idasen.SystemTray.Settings
{
    public class CommonApplicationData : ICommonApplicationData
    {
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
        public string ToFullPath(string fileName)
        {
            var fullPath = Path.Combine(FolderName(),
                                        fileName);
            return fullPath;
        }

        public string FolderName()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            var folderName = Path.Combine(appData,
                                          Constants.ApplicationName);

            return folderName;
        }
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
    }
}