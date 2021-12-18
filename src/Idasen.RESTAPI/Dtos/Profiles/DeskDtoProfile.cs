using AutoMapper ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.RESTAPI.Dtos.Profiles
{
    [ UsedImplicitly ]
    internal class DeskDtoProfile
        : Profile
    {
        public DeskDtoProfile ( )
        {
            CreateMap < IDesk , DeskDto > ( )
               .ForMember ( dest => dest.BluetoothAddress ,
                            opt => opt.MapFrom ( src => src.BluetoothAddress ) )
               .ForMember ( dest => dest.BluetoothAddressType ,
                            opt => opt.MapFrom ( src => src.BluetoothAddressType ) )
               .ForMember ( dest => dest.DeviceName ,
                            opt => opt.MapFrom ( src => src.DeviceName ) )
               .ForMember ( dest => dest.Name ,
                            opt => opt.MapFrom ( src => src.Name ) ) ;
        }
    }
}