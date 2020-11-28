using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [ TestClass ]
    public class ReferenceOutputTests
    {
        [ TestMethod ]
        public void GattServiceUuid_ForInvoked_Empty ( )
        {
            CreateSut ( ).GattServiceUuid
                         .Should ( )
                         .Be ( Guid.Empty ) ;
        }

        [ TestMethod ]
        public void RawHeightSpeed_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawHeightSpeed
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawTwo_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawTwo
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawThree_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawThree
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawFour_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawFour
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawFive_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawFive
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawSix_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawSix
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawSeven_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawSeven
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawEight_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawEight
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawMask_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawMask
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void RawDetectMask_ForInvoked_Empty ( )
        {
            CreateSut ( ).RawDetectMask
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void HeightSpeedChanged_ForInvoked_Throws ( )
        {
            Action action = ( ) => CreateSut ( ).HeightSpeedChanged
                                                .Subscribe ( ) ;

            action.Should ( )
                  .Throw < NotInitializeException > ( ) ;
        }

        [ TestMethod ]
        public void Dispose_ForInvoked_DoesNothing ( )
        {
            Action action = ( ) => CreateSut ( ).Dispose ( ) ;

            action.Should ( )
                  .NotThrow < Exception > ( ) ;
        }

        private ReferenceOutput CreateSut ( )
        {
            return new ReferenceOutput ( ) ;
        }
    }
}