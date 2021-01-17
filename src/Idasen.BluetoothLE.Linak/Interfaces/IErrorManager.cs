using System ;
using System.Runtime.CompilerServices ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IErrorManager
    {
        /// <summary>
        ///     Notify when an error happened.
        /// </summary>
        IObservable < IErrorDetails > ErrorChanged { get ; }

        /// <summary>
        ///     Publish an error so the UI can show it.
        /// </summary>
        /// <param name="details">
        ///     The details about the error.
        /// </param>
        void Publish ( [ NotNull ] IErrorDetails details ) ;

        /// <inheritdoc />
        void PublishForMessage ( [ NotNull ]          string message ,
                                 [ CallerMemberName ] string caller = "" ) ;
    }
}