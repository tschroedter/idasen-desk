using System ;
using Idasen.BluetoothLE.Core ;
using JetBrains.Annotations ;
using Selkie.DefCon.One.Common ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    public class ResourceNotFoundException
        : Exception // todo duplicated code
    {
        public ResourceNotFoundException (
            [ NotNull ]                   string resourceName ,
            [ CanBeNull ] [ GuardIgnore ] string message )
            : base ( message )
        {
            Guard.ArgumentNotNull ( resourceName ,
                                    nameof ( resourceName ) ) ;
            ResourceName = resourceName ;
        }

        public string ResourceName { get ; }
    }
}