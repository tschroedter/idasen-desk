using System ;
using System.Globalization ;
using CsvHelper ;
using CsvHelper.Configuration ;
using CsvHelper.TypeConversion ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    public class OfficialGattServiceConverter
        : DefaultTypeConverter
    {
        public override object ConvertFromString (
            [ NotNull ] string        text ,
            [ NotNull ] IReaderRow    readerRow ,
            [ NotNull ] MemberMapData memberMapData )
        {
            Guard.ArgumentNotNull ( text ,
                                    nameof ( text ) ) ;
            Guard.ArgumentNotNull ( readerRow ,
                                    nameof ( readerRow ) ) ;
            Guard.ArgumentNotNull ( memberMapData ,
                                    nameof ( memberMapData ) ) ;

            var number = text.Replace ( "0x" ,
                                        "" ,
                                        StringComparison.InvariantCulture ) ;

            return ushort.TryParse ( number ,
                                     NumberStyles.HexNumber ,
                                     CultureInfo.InvariantCulture ,
                                     out var value )
                       ? value
                       : ushort.MaxValue ;
        }

        public override string ConvertToString (
            [ NotNull ] object        value ,
            [ NotNull ] IWriterRow    writerRow ,
            [ NotNull ] MemberMapData memberMapData )
        {
            Guard.ArgumentNotNull ( value ,
                                    nameof ( value ) ) ;
            Guard.ArgumentNotNull ( writerRow ,
                                    nameof ( writerRow ) ) ;
            Guard.ArgumentNotNull ( memberMapData ,
                                    nameof ( memberMapData ) ) ;

            return value.ToString ( ) ;
        }
    }
}