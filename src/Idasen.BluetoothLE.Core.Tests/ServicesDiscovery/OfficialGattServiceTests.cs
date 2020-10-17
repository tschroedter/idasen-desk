using FluentAssertions ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery
{
    [ TestClass ]
    public class OfficialGattServiceTests
    {
        public const string Name                  = nameof ( Name ) ;
        public const string UniformTypeIdentifier = nameof ( UniformTypeIdentifier ) ;
        public const ushort AssignedNumber        = 1 ;
        public const string ProfileSpecification  = nameof ( ProfileSpecification ) ;

        [ TestMethod ]
        public void Constructor_ForInvoke_SetsName ( )
        {
            CreateSut ( ).Name
                         .Should ( )
                         .Be ( Name ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoke_SetsUniformTypeIdentifier ( )
        {
            CreateSut ( ).UniformTypeIdentifier
                         .Should ( )
                         .Be ( UniformTypeIdentifier ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoke_SetsAssignedNumber ( )
        {
            CreateSut ( ).AssignedNumber
                         .Should ( )
                         .Be ( AssignedNumber ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoke_SetsProfileSpecification ( )
        {
            CreateSut ( ).ProfileSpecification
                         .Should ( )
                         .Be ( ProfileSpecification ) ;
        }

        private static OfficialGattService CreateSut ( )
        {
            return new OfficialGattService
                   {
                       Name                  = Name ,
                       UniformTypeIdentifier = UniformTypeIdentifier ,
                       AssignedNumber        = AssignedNumber ,
                       ProfileSpecification  = ProfileSpecification
                   } ;
        }
    }
}