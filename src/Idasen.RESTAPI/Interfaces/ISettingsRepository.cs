using System.Threading.Tasks ;
using Idasen.RESTAPI.Dtos ;

namespace Idasen.RESTAPI.Interfaces
{
    public interface ISettingsRepository
    {
        Task < bool >        InsertSettings ( SettingsDto dto ) ;
        Task < SettingsDto > GetSettingsById ( string     id ) ;
        Task < SettingsDto > GetDefaultSettings ( string  id ) ;
    }
}