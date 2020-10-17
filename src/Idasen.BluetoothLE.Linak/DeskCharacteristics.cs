using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories;
using Idasen.BluetoothLE.Linak.Interfaces;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using Serilog;

namespace Idasen.BluetoothLE.Linak
{
    public class DeskCharacteristics
        : IDeskCharacteristics
    {
        [JetBrains.Annotations.NotNull] private readonly IControlFactory          _controlFactory;
        [JetBrains.Annotations.NotNull] private readonly IDpgFactory              _dpgFactory;
        [JetBrains.Annotations.NotNull] private readonly IGenericAccessFactory    _genericAccessFactory;
        [JetBrains.Annotations.NotNull] private readonly IGenericAttributeFactory _genericAttributeFactory;
        [JetBrains.Annotations.NotNull] private readonly ILogger                  _logger;
        [JetBrains.Annotations.NotNull] private readonly IReferenceInputFactory   _referenceInputFactory;
        [JetBrains.Annotations.NotNull] private readonly IReferenceOutputFactory  _referenceOutputFactory;

        [SuppressMessage("NDepend",
                         "ND1004:AvoidMethodsWithTooManyParameters",
                         Justification = "The real desk contains all the GATT characteristics.")]
        public DeskCharacteristics(
            [JetBrains.Annotations.NotNull] ILogger                  logger,
            [JetBrains.Annotations.NotNull] IGenericAccessFactory    genericAccessFactory,
            [JetBrains.Annotations.NotNull] IGenericAttributeFactory genericAttributeFactory,
            [JetBrains.Annotations.NotNull] IReferenceInputFactory   referenceInputFactory,
            [JetBrains.Annotations.NotNull] IReferenceOutputFactory  referenceOutputFactory,
            [JetBrains.Annotations.NotNull] IDpgFactory              dpgFactory,
            [JetBrains.Annotations.NotNull] IControlFactory          controlFactory)
        {
            Guard.ArgumentNotNull(logger,
                                  nameof(logger));
            Guard.ArgumentNotNull(genericAccessFactory,
                                  nameof(genericAccessFactory));
            Guard.ArgumentNotNull(genericAttributeFactory,
                                  nameof(genericAttributeFactory));
            Guard.ArgumentNotNull(referenceInputFactory,
                                  nameof(referenceInputFactory));
            Guard.ArgumentNotNull(referenceOutputFactory,
                                  nameof(referenceOutputFactory));
            Guard.ArgumentNotNull(dpgFactory,
                                  nameof(dpgFactory));
            Guard.ArgumentNotNull(controlFactory,
                                  nameof(controlFactory));

            _logger                  = logger;
            _genericAccessFactory    = genericAccessFactory;
            _genericAttributeFactory = genericAttributeFactory;
            _referenceInputFactory   = referenceInputFactory;
            _referenceOutputFactory  = referenceOutputFactory;
            _dpgFactory              = dpgFactory;
            _controlFactory          = controlFactory;
        }

        public IDevice Device { get; } = new Characteristics.Characteristics.Unknowns.Device();

        public IGenericAccess    GenericAccess    { get; private set; } = new Characteristics.Characteristics.Unknowns.GenericAccess();
        public IGenericAttribute GenericAttribute { get; private set; } = new Characteristics.Characteristics.Unknowns.GenericAttribute();
        public IReferenceInput   ReferenceInput   { get; private set; } = new Characteristics.Characteristics.Unknowns.ReferenceInput();
        public IReferenceOutput  ReferenceOutput  { get; private set; } = new Characteristics.Characteristics.Unknowns.ReferenceOutput();
        public IDpg              Dpg              { get; private set; } = new Characteristics.Characteristics.Unknowns.Dpg();
        public IControl          Control          { get; private set; } = new Characteristics.Characteristics.Unknowns.Control();

        public async Task Refresh()
        {
            _logger.Debug($"[{Device}] Refreshing characteristics...");

            await GenericAccess.Refresh();
            await GenericAttribute.Refresh();
            await ReferenceInput.Refresh();
            await ReferenceOutput.Refresh();
            await Dpg.Refresh();
            await Control.Refresh();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(GenericAccess.ToString());
            builder.AppendLine(GenericAttribute.ToString());
            builder.AppendLine(ReferenceInput.ToString());
            builder.AppendLine(ReferenceOutput.ToString());
            builder.AppendLine(Dpg.ToString());
            builder.AppendLine(Control.ToString());

            return builder.ToString();
        }

        public IDeskCharacteristics Initialize(IDevice device)
        {
            Guard.ArgumentNotNull(device,
                                  nameof(device));

            GenericAccess = _genericAccessFactory.Create(device);
            GenericAccess.Initialize<Characteristics.Characteristics.GenericAccess>();

            GenericAttribute = _genericAttributeFactory.Create(device);
            GenericAttribute.Initialize<Characteristics.Characteristics.GenericAttribute>();

            ReferenceInput = _referenceInputFactory.Create(device);
            ReferenceInput.Initialize<Characteristics.Characteristics.GenericAttribute>();

            ReferenceOutput = _referenceOutputFactory.Create(device);
            ReferenceOutput.Initialize<Characteristics.Characteristics.ReferenceOutput>();

            Dpg = _dpgFactory.Create(device);
            Dpg.Initialize<Characteristics.Characteristics.Dpg>();

            Control = _controlFactory.Create(device);
            Control.Initialize<Characteristics.Characteristics.Control>();

            return this;
        }
    }
}