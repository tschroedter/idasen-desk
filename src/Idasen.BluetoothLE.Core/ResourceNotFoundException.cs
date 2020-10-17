using System ;
using JetBrains.Annotations ;
using Selkie.DefCon.One.Common ;

namespace Idasen.BluetoothLE.Core
{
    public class ResourceNotFoundException : Exception
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