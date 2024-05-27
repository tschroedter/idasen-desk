using System ;
using System.IO ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils ;

namespace Idasen.SystemTray.Settings ;

public class CommonApplicationData : ICommonApplicationData
{
    private readonly Lazy<string> _folderName;
    
    public CommonApplicationData()
    {
        _folderName = new Lazy < string > ( FolderName ) ;
    }
    
    public string ToFullPath(string fileName)
    {
        return Path.Combine ( _folderName.Value ,
                                           fileName ) ;
    }

    public string FolderName ( )
    {
        var appData = Environment.GetFolderPath ( Environment.SpecialFolder.CommonApplicationData ) ;

        return Path.Combine ( appData , Constants.ApplicationName ) ;
    }
}