using System ;
using System.Collections.Generic ;
using System.Diagnostics.CodeAnalysis ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ ExcludeFromCodeCoverage ]
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class GattCharacteristicsResultWrapper
        : IGattCharacteristicsResultWrapper
    {
        public GattCharacteristicsResultWrapper (
            [ NotNull ] IGattCharacteristicWrapperFactory factory ,
            [ NotNull ] GattCharacteristicsResult         result )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            _factory = factory ;
            _result  = result ;
        }

        /// <inheritdoc />
        public GattCommunicationStatus Status => _result.Status ;

        /// <inheritdoc />
        public byte? ProtocolError => _result.ProtocolError ;

        /// <inheritdoc />
        public IReadOnlyList < IGattCharacteristicWrapper > Characteristics { get ; private set ; }

        public async Task < IGattCharacteristicsResultWrapper > Initialize ( )
        {
            var wrappers = _result.Characteristics
                                  .Select ( characteristic => _factory.Create ( characteristic ) )
                                  .ToList ( ) ;

            foreach ( var wrapper in wrappers )
            {
                await wrapper.Initialize ( ) ;
            }

            Characteristics = wrappers ;

            return this ;
        }

        public delegate IGattCharacteristicsResultWrapper Factory ( GattCharacteristicsResult result ) ;

        public override string ToString ( )
        {
            var builder = new StringBuilder ( ) ;

            builder.Append ( CharacteristicsToString ( ) ) ;

            return builder.ToString ( ) ;
        }

        private string CharacteristicsToString ( )
        {
            var list = new List < string > ( ) ;

            foreach ( var characteristic in Characteristics )
            {
                var properties = characteristic.CharacteristicProperties.ToCsv ( ) ;
                var formats    = PresentationFormatsToString ( characteristic.PresentationFormats ) ;

                list.Add ( $"Service UUID = {characteristic.ServiceUuid} "         +
                           $"Characteristic UUID = {characteristic.Uuid}, "        +
                           $"UserDescription = {characteristic.UserDescription}, " +
                           $"ProtectionLevel = {characteristic.ProtectionLevel}, " +
                           $"AttributeHandle = {characteristic.AttributeHandle}, " +
                           $"CharacteristicProperties = [{properties}] "           +
                           $"PresentationFormats = [{formats}]" ) ;
            }

            return string.Join ( Environment.NewLine ,
                                 list ) ;
        }

        private static string PresentationFormatsToString (
            IReadOnlyList < GattPresentationFormat > characteristicPresentationFormats )
        {
            var list = new List < string > ( ) ;

            foreach ( var format in characteristicPresentationFormats )
            {
                list.Add ( "GattPresentationFormat - "             +
                           $"Description = {format.Description}, " +
                           $"FormatType = {format.FormatType}, "   +
                           $"Namespace = {format.Namespace}, "     +
                           $"Exponent = {format.Exponent}, "       +
                           $"Unit = {format.Unit}" ) ;
            }

            return string.Join ( ", " ,
                                 list ) ;
        }

        private readonly IGattCharacteristicWrapperFactory _factory ;
        private readonly GattCharacteristicsResult         _result ;
    }
}