using System.Collections.Generic ;
using System.Linq ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public class UnknownBase
        : ICharacteristicBase
    {
        public T Initialize < T > ( ) where T : class
        {
            return this as T ;
        }

        public Task Refresh ( )
        {
            return Task.FromResult ( false ) ;
        }

        public void Dispose ( )
        {
            Dispose ( true ) ;
        }

        protected static readonly IEnumerable < byte > RawArrayEmpty = Enumerable.Empty < byte > ( )
                                                                                 .ToArray ( ) ;

        protected virtual void Dispose ( bool disposing )
        {
            if ( _disposed ) return ;

            _disposed = true ;
        }

        private bool _disposed ;
    }
}