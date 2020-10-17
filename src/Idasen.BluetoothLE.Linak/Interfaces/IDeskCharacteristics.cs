using System.Threading.Tasks;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using JetBrains.Annotations;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskCharacteristics
    {
        IGenericAccess       GenericAccess    { get; }
        IGenericAttribute    GenericAttribute { get; }
        IReferenceInput      ReferenceInput   { get; }
        IReferenceOutput     ReferenceOutput  { get; }
        IDpg                 Dpg              { get; }
        IControl             Control          { get; }
        IDeskCharacteristics Initialize([NotNull] IDevice device);
        Task                 Refresh();
    }
}