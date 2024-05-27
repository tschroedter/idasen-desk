namespace Idasen.SystemTray.Interfaces
{
    public interface ICommonApplicationData
    {
        /// <summary>
        ///     Create the full path to the given file in the 'ProgramData' folder.
        /// </summary>
        /// <param name="fileName">
        ///     The filename.
        /// </param>
        /// <returns>
        ///     The full path to the file in the 'ProgramData' folder.
        /// </returns>
        string ToFullPath(string fileName);

        /// <summary>
        ///     Gets the 'ProgramData' folder for the application.
        /// </summary>
        /// <returns>
        ///     The 'ProgramData' folder for the application.
        /// </returns>
        string FolderName ( ) ;
    }
}