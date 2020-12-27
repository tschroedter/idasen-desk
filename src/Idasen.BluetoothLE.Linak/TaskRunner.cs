using System ;
using System.Diagnostics.CodeAnalysis ;
using System.Threading ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;

namespace Idasen.BluetoothLE.Linak
{
    [ ExcludeFromCodeCoverage ]
    public class TaskRunner
        : ITaskRunner
    {
        public Task Run (
            [ JetBrains.Annotations.NotNull ] Action action ,
            CancellationToken                        token )
        {
            Guard.ArgumentNotNull ( action ,
                                    nameof ( action ) ) ;
            return Task.Run ( action ,
                              token ) ;
        }
    }
}