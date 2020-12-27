using System ;
using System.Collections.Generic ;
using System.Globalization ;
using System.IO ;
using System.Linq ;
using System.Reflection ;
using CsvHelper ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    public sealed class AllGattCharacteristicsProvider
        : IAllGattCharacteristicsProvider
    {
        public AllGattCharacteristicsProvider ( )
        {
            OfficialGattCharacteristics = GetType ( ).Namespace +
                                          "."                   +
                                          Filename ;

            Populate ( ReadCsvFile ( ) ) ;
        }

        public bool TryGetDescription ( Guid       uuid ,
                                        out string description )
        {
            return _uuidToDescription.TryGetValue ( uuid ,
                                                    out description ) ;
        }

        public bool TryGetUuid ( string   description ,
                                 out Guid uuid )
        {
            return _descriptionToUuid.TryGetValue ( description ,
                                                    out uuid ) ;
        }

        internal const string Filename = "OfficialGattCharacteristics.txt" ;

        public string OfficialGattCharacteristics { get ; }

        private void Populate ( IEnumerable < CsvGattCharacteristic > records )
        {
            foreach ( var record in records )
            {
                _uuidToDescription [ Guid.Parse ( record.Uuid ) ] = record.Name ;
            }

            foreach ( var (uuid , description) in _uuidToDescription )
            {
                _descriptionToUuid [ description ] = uuid ;
            }
        }

        private IEnumerable < CsvGattCharacteristic > ReadCsvFile ( )
        {
            var stream = Assembly.GetExecutingAssembly ( )
                                 .GetManifestResourceStream ( OfficialGattCharacteristics ) ;

            if ( stream == null )
                throw new ResourceNotFoundException ( OfficialGattCharacteristics ,
                                                      $"Can't find resource {OfficialGattCharacteristics}" ) ;

            using var reader = new StreamReader ( stream ) ;

            using var csv = new CsvReader ( reader ,
                                            CultureInfo.InvariantCulture ) ;

            csv.Configuration.Delimiter = "," ;

            var readCsvFile = csv.GetRecords < CsvGattCharacteristic > ( )
                                 .ToArray ( ) ;

            return readCsvFile ;
        }

        private readonly Dictionary < string , Guid > _descriptionToUuid = new Dictionary < string , Guid > ( ) ;
        private readonly Dictionary < Guid , string > _uuidToDescription = new Dictionary < Guid , string > ( ) ;
    }
}