using System ;
using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery
{
    [ AutoDataTestClass ]
    public class MatchMakerTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForKnownAddress_Instance (
            MatchMaker sut ,
            ulong      address )
        {
            sut.PairToDeviceAsync ( address )
               .Should ( )
               .NotBeNull ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Create_ForUnknownAddress_Throws (
            Lazy < MatchMaker >       sut ,
            [ Freeze ] IDeviceFactory deviceFactory ,
            ulong                     address )
        {
            deviceFactory.FromBluetoothAddressAsync ( Arg.Any < ulong > ( ) )
                         .Returns ( ( IDevice ) null ) ;

            Func < Task > action = async ( ) =>
                                   {
                                       await sut.Value
                                                .PairToDeviceAsync ( address )
                                                .ConfigureAwait ( false ) ;
                                   } ;

            action.Should ( )
                  .ThrowAsync< ArgumentNullException > ( ) ;
        }
    }
}