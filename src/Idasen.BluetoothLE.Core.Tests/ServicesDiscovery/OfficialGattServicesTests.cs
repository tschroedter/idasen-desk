using System ;
using System.Collections.Generic ;
using FluentAssertions ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery
{
    [ TestClass ]
    public class OfficialGattServicesTests
    {
        [ TestMethod ]
        public void Constructor_ForInvoke_SetsResourceName ( )
        {
            var expected = "Idasen.BluetoothLE.Core.ServicesDiscovery.OfficialGattServices.txt" ;

            CreateSut ( ).ResourceName
                         .Should ( )
                         .Be ( expected ) ;
        }

        [ TestMethod ]
        public void Count_ForInvoke_ReturnsCorrectNumber ( )
        {
            var expected = CreateUuidCollection ( ).Count ;

            CreateSut ( ).Count
                         .Should ( )
                         .Be ( expected ) ;
        }

        [ TestMethod ]
        public void TryFindByUuid_ForUnknownUuid_ReturnsFalse ( )
        {
            CreateSut ( ).TryFindByUuid ( Guid.Empty ,
                                          out _ )
                         .Should ( )
                         .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void TryFindByUuid_ForUnknownUuid_ReturnsNull ( )
        {
            CreateSut ( ).TryFindByUuid ( Guid.Empty ,
                                          out var gattService ) ;

            gattService.Should ( )
                       .BeNull ( ) ;
        }

        [ TestMethod ]
        public void TryFindByUuid_ForKnownUuid_ReturnsTrue ( )
        {
            CreateSut ( ).TryFindByUuid ( _knownGuid ,
                                          out _ )
                         .Should ( )
                         .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void TryFindByUuid_ForKnownUuid_ReturnsGattService ( )
        {
            CreateSut ( ).TryFindByUuid ( _knownGuid ,
                                          out var gattService ) ;

            gattService.Should ( )
                       .NotBeNull ( ) ;
        }

        [ TestMethod ]
        public void TryFindByUuid_ForAllKnownUUIDs_ReturnsGattService ( )
        {
            var sut = CreateSut ( ) ;

            foreach ( var assignedNumber in CreateUuidCollection ( ) )
            {
                var knownGuid = Guid.Parse ( "0000"                          +
                                             assignedNumber.ToString ( "X" ) +
                                             "-0000-1000-8000-00805f9b34fb" ) ;

                sut.TryFindByUuid ( knownGuid ,
                                    out var gattService ) ;

                gattService.Should ( )
                           .NotBeNull ( ) ;
            }
        }

        [ TestMethod ]
        public void GetEnumerator_ForInvoke_ReturnsCollection ( )
        {
            var expected = CreateUuidCollection ( ).Count ;

            var count = CreateSut ( ).Count ;

            count.Should ( )
                 .Be ( expected ) ;
        }

        private IReadOnlyCollection < ushort > CreateUuidCollection ( )
        {
            return new [ ]
                   {
                       ( ushort ) 0x1800 ,
                       ( ushort ) 0x1811 ,
                       ( ushort ) 0x1815 ,
                       ( ushort ) 0x180F ,
                       ( ushort ) 0x183B ,
                       ( ushort ) 0x1810 ,
                       ( ushort ) 0x181B ,
                       ( ushort ) 0x181E ,
                       ( ushort ) 0x181F ,
                       ( ushort ) 0x1805 ,
                       ( ushort ) 0x1818 ,
                       ( ushort ) 0x1816 ,
                       ( ushort ) 0x180A ,
                       ( ushort ) 0x183C ,
                       ( ushort ) 0x181A ,
                       ( ushort ) 0x1826 ,
                       ( ushort ) 0x1801 ,
                       ( ushort ) 0x1808 ,
                       ( ushort ) 0x1809 ,
                       ( ushort ) 0x180D ,
                       ( ushort ) 0x1823 ,
                       ( ushort ) 0x1812 ,
                       ( ushort ) 0x1802 ,
                       ( ushort ) 0x1821 ,
                       ( ushort ) 0x183A ,
                       ( ushort ) 0x1820 ,
                       ( ushort ) 0x1803 ,
                       ( ushort ) 0x1819 ,
                       ( ushort ) 0x1827 ,
                       ( ushort ) 0x1828 ,
                       ( ushort ) 0x1807 ,
                       ( ushort ) 0x1825 ,
                       ( ushort ) 0x180E ,
                       ( ushort ) 0x1822 ,
                       ( ushort ) 0x1829 ,
                       ( ushort ) 0x1806 ,
                       ( ushort ) 0x1814 ,
                       ( ushort ) 0x1813 ,
                       ( ushort ) 0x1824 ,
                       ( ushort ) 0x1804 ,
                       ( ushort ) 0x181C ,
                       ( ushort ) 0x181D
                   } ;
        }

        private static OfficialGattServices CreateSut ( )
        {
            return new OfficialGattServices ( ) ;
        }

        private readonly Guid _knownGuid = Guid.Parse ( "00001800-0000-1000-8000-00805f9b34fb" ) ;
    }
}