using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    /// <inheritdoc />
    public class DeskCharacteristicsCreator
        : IDeskCharacteristicsCreator
    {
        public DeskCharacteristicsCreator (
            [ NotNull ] ILogger                  logger ,
            [ NotNull ] IGenericAccessFactory    genericAccessFactory ,
            [ NotNull ] IGenericAttributeFactory genericAttributeFactory ,
            [ NotNull ] IReferenceInputFactory   referenceInputFactory ,
            [ NotNull ] IReferenceOutputFactory  referenceOutputFactory ,
            [ NotNull ] IDpgFactory              dpgFactory ,
            [ NotNull ] IControlFactory          controlFactory )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( genericAccessFactory ,
                                    nameof ( genericAccessFactory ) ) ;
            Guard.ArgumentNotNull ( genericAttributeFactory ,
                                    nameof ( genericAttributeFactory ) ) ;
            Guard.ArgumentNotNull ( referenceInputFactory ,
                                    nameof ( referenceInputFactory ) ) ;
            Guard.ArgumentNotNull ( referenceOutputFactory ,
                                    nameof ( referenceOutputFactory ) ) ;
            Guard.ArgumentNotNull ( dpgFactory ,
                                    nameof ( dpgFactory ) ) ;
            Guard.ArgumentNotNull ( controlFactory ,
                                    nameof ( controlFactory ) ) ;

            _logger                  = logger ;
            _genericAccessFactory    = genericAccessFactory ;
            _genericAttributeFactory = genericAttributeFactory ;
            _referenceInputFactory   = referenceInputFactory ;
            _referenceOutputFactory  = referenceOutputFactory ;
            _dpgFactory              = dpgFactory ;
            _controlFactory          = controlFactory ;
        }

        /// <inheritdoc />
        public void Create (
            IDeskCharacteristics characteristics ,
            IDevice              device )
        {
            Guard.ArgumentNotNull ( device ,
                                    nameof ( device ) ) ;
            Guard.ArgumentNotNull ( characteristics ,
                                    nameof ( characteristics ) ) ;
            Guard.ArgumentNotNull ( device ,
                                    nameof ( device ) ) ;

            _logger.Debug ( $"[{device.Id}] Creating desk characteristics {characteristics}" );

            characteristics.WithCharacteristics ( DeskCharacteristicKey.GenericAccess ,
                                                  _genericAccessFactory.Create ( device ) )
                           .WithCharacteristics ( DeskCharacteristicKey.GenericAttribute ,
                                                  _genericAttributeFactory.Create ( device ) )
                           .WithCharacteristics ( DeskCharacteristicKey.ReferenceInput ,
                                                  _referenceInputFactory.Create ( device ) )
                           .WithCharacteristics ( DeskCharacteristicKey.ReferenceOutput ,
                                                  _referenceOutputFactory.Create ( device ) )
                           .WithCharacteristics ( DeskCharacteristicKey.Dpg ,
                                                  _dpgFactory.Create ( device ) )
                           .WithCharacteristics ( DeskCharacteristicKey.Control ,
                                                  _controlFactory.Create ( device ) ) ;
        }

        private readonly IControlFactory          _controlFactory ;
        private readonly IDpgFactory              _dpgFactory ;
        private readonly IGenericAccessFactory    _genericAccessFactory ;
        private readonly IGenericAttributeFactory _genericAttributeFactory ;
        private readonly ILogger                  _logger ;
        private readonly IReferenceInputFactory   _referenceInputFactory ;
        private readonly IReferenceOutputFactory  _referenceOutputFactory ;
    }
}