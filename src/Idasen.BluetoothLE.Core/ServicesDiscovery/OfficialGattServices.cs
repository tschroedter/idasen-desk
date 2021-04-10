using System ;
using System.Collections ;
using System.Collections.Generic ;
using System.Diagnostics.CodeAnalysis ;
using System.Globalization ;
using System.IO ;
using System.Linq ;
using System.Reflection ;
using Autofac.Extras.DynamicProxy ;
using CsvHelper ;
using CsvHelper.Configuration ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    /// <inheritdoc />
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class OfficialGattServices
        : IOfficialGattServices
    {
        public OfficialGattServices ( )
        {
            ResourceName = GetType ( ).Namespace + "." + FileName ;

            Populate ( ReadCsvFile ( ResourceName ) ) ;
        }

        /// <inheritdoc />
        public IEnumerator < OfficialGattService > GetEnumerator ( )
        {
            return _dictionary.Values.GetEnumerator ( ) ;
        }

        /// <inheritdoc />
        [ ExcludeFromCodeCoverage ]
        IEnumerator IEnumerable.GetEnumerator ( )
        {
            return GetEnumerator ( ) ;
        }

        /// <inheritdoc />
        public int Count => _dictionary.Count ;

        /// <inheritdoc />
        public bool TryFindByUuid ( Guid                    guid ,
                                    out OfficialGattService gattService )
        {
            gattService = null ;

            var number = guid.ToString ( "N" )
                             .Substring ( 4 ,
                                          4 ) ;

            ushort assignedNumber ;

            try
            {
                assignedNumber = ( ushort ) Convert.ToInt32 ( number ,
                                                              16 ) ;
            }
            catch ( Exception )
            {
                return false ;
            }

            return _dictionary.TryGetValue ( assignedNumber ,
                                             out gattService ) ;
        }

        private const string FileName = "OfficialGattServices.txt" ;

        public string ResourceName { get ; }

        private void Populate ( IEnumerable < OfficialGattService > records )
        {
            foreach ( var record in records )
            {
                _dictionary [ record.AssignedNumber ] = record ;
            }
        }

        private static IEnumerable < OfficialGattService > ReadCsvFile ( string resourceName )
        {
            var stream = Assembly.GetExecutingAssembly ( )
                                 .GetManifestResourceStream ( resourceName ) ;

            if ( stream == null )
                throw new ResourceNotFoundException ( resourceName ,
                                                      $"Can't find resource {resourceName}" ) ;

            using var reader = new StreamReader ( stream ) ;

            var config = new CsvConfiguration ( CultureInfo.InvariantCulture )
                         {
                             Delimiter = "," ,
                             HasHeaderRecord = true
                         } ;

            using var csv = new CsvReader ( reader ,
                                            config ) ;


            csv.Context.RegisterClassMap < GattServiceCsvHelperMap > ( ) ;

            var readCsvFile = csv.GetRecords < OfficialGattService > ( )
                                 .ToArray ( ) ;

            return readCsvFile ;
        }

        private readonly Dictionary < ushort , OfficialGattService > _dictionary =
            new Dictionary < ushort , OfficialGattService > ( ) ;
    }
}