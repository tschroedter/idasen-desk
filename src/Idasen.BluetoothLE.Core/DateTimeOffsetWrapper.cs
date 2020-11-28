using System ;
using System.Diagnostics.CodeAnalysis ;
using Idasen.BluetoothLE.Core.Interfaces ;

namespace Idasen.BluetoothLE.Core
{
    /// <inheritdoc />
    [ ExcludeFromCodeCoverage ]
    public class DateTimeOffsetWrapper
        : IDateTimeOffset
    {
        public DateTimeOffsetWrapper ( )
            : this ( new DateTimeOffset ( ) )
        {
        }

        public DateTimeOffsetWrapper ( DateTimeOffset dateTimeOffset )
        {
            _dateTimeOffset = dateTimeOffset ;
        }

        /// <inheritdoc />
        public IDateTimeOffset Now => new DateTimeOffsetWrapper ( DateTimeOffset.Now ) ;

        /// <inheritdoc />
        public long Ticks => _dateTimeOffset.Ticks ;

        /// <inheritdoc />
        public string ToString ( [ JetBrains.Annotations.NotNull ] string          format ,
                                 [ JetBrains.Annotations.NotNull ] IFormatProvider formatProvider )
        {
            Guard.ArgumentNotNull ( format ,
                                    nameof ( format ) ) ;
            Guard.ArgumentNotNull ( formatProvider ,
                                    nameof ( formatProvider ) ) ;

            return _dateTimeOffset.ToString ( format ,
                                              formatProvider ) ;
        }

        /// <inheritdoc />
        public override string ToString ( )
        {
            return _dateTimeOffset.ToString ( ) ;
        }

        private readonly DateTimeOffset _dateTimeOffset ;
    }
}