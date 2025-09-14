using System.IO.Abstractions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class CommonApplicationData : ICommonApplicationData
{
    private readonly IFileSystem     _fileSystem ;
    private readonly Lazy < string > _folderName ;

    public CommonApplicationData ( ) : this ( new FileSystem ( ) ) { }

    public CommonApplicationData ( IFileSystem fileSystem )
    {
        _fileSystem = fileSystem ;
        _folderName = new Lazy < string > ( FolderName ) ;
    }

    public string ToFullPath ( string fileName )
    {
        var folder = _folderName.Value ;

        if ( ! _fileSystem.Directory.Exists ( folder ) )
            _fileSystem.Directory.CreateDirectory ( folder ) ;

        return _fileSystem.Path.Combine ( folder ,
                                          fileName ) ;
    }

    public string FolderName ( )
    {
        var appData = Environment.GetFolderPath ( Environment.SpecialFolder.CommonApplicationData ) ;

        return _fileSystem.Path.Combine ( appData ,
                                          Constants.ApplicationName ) ;
    }
}