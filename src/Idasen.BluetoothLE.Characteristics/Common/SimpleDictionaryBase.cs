using System.Collections.Generic ;
using System.Linq ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    public class SimpleDictionaryBase < TKey , TValue >
        : ISimpleDictionary < TKey , TValue >
    {
        /// <inheritdoc />
        public TValue this [ TKey key ]
        {
            get
            {
                lock ( _padlock )
                {
                    return _dictionary [ key ] ;
                }
            }
            set
            {
                Guard.ArgumentNotNull ( value ,
                                        nameof ( value ) ) ;

                lock ( _padlock )
                {
                    _dictionary [ key ] = value ;
                }
            }
        }

        /// <inheritdoc />
        public void Clear ( )
        {
            lock ( _padlock )
            {
                _dictionary.Clear ( ) ;
            }
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock ( _padlock )
                {
                    return _dictionary.Count ;
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable < string > Keys
        {
            get
            {
                lock ( _padlock )
                {
                    return _dictionary.Keys
                                      .Cast < string > ( )
                                      .Where ( x => x != null )
                                      .ToArray ( ) ;
                }
            }
        }

        /// <inheritdoc />
        public IReadOnlyDictionary < TKey , TValue > ReadOnlyDictionary
        {
            get
            {
                lock ( _padlock )
                {
                    return _dictionary.ToDictionary ( item => item.Key ,
                                                      item => item.Value ) ;
                }
            }
        }

        private readonly Dictionary < TKey , TValue > _dictionary =
            new Dictionary < TKey , TValue > ( ) ;

        private readonly object _padlock = new object ( ) ;
    }
}