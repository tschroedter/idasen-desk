using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ TestClass ]
    public class StoppingHeightCalculatorTests
    {
        [ TestInitialize ]
        public void Initialize ( )
        {
            _logger     = Substitute.For < ILogger > ( ) ;
            _calculator = new HasReachedTargetHeightCalculator ( _logger ) ;
        }

        [ TestMethod ]
        public void Constructor_ForLoggerNull_Throws ( )
        {
            _logger = null ;

            Action action = ( ) => CreateSut ( ) ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "logger" ) ;
        }

        [ TestMethod ]
        public void Constructor_ForCalculatorNull_Throws ( )
        {
            _calculator = null ;

            Action action = ( ) => CreateSut ( ) ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "calculator" ) ;
        }

        [ DataRow ( 2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1000u ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1020u ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    980u ) ]
        [ DataTestMethod ]
        public void Calculate_ForParameters_SetsStoppingHeight (
            uint  targetHeight ,
            uint  height ,
            int   speed ,
            int   maxSpeed ,
            uint  maxMovement ,
            float fudgeFactor ,
            uint  stoppingHeight )
        {
            var sut = CreateSut ( ) ;

            sut.TargetHeight           = targetHeight ;
            sut.Height                 = height ;
            sut.Speed                  = speed ;
            sut.MaxSpeed               = maxSpeed ;
            sut.MaxSpeedToStopMovement = maxMovement ;
            sut.FudgeFactor            = fudgeFactor ;

            sut.Calculate ( )
               .StoppingHeight
               .Should ( )
               .Be ( stoppingHeight ) ;
        }

        [ DataRow ( Direction.None ,
                    Direction.Up ,
                    2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1000u ) ]
        [ DataRow ( Direction.None ,
                    Direction.Down ,
                    2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1000u ) ]
        [ DataRow ( Direction.None ,
                    Direction.None ,
                    2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1000u ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1000u ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    2000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    980u ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    2000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1020u ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    1000u ,
                    2000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1000u ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    1000u ,
                    2000u ,
                    100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    1020u ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    1000u ,
                    2000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    980u ) ]
        [ DataTestMethod ]
        public void Calculate_ForParameters_SetsDelta (
            Direction moveIntoDirection ,
            Direction startMovingIntoDirection ,
            uint      targetHeight ,
            uint      height ,
            int       speed ,
            int       maxSpeed ,
            uint      maxMovement ,
            float     fudgeFactor ,
            uint      delta )
        {
            var sut = CreateSut ( ) ;

            sut.TargetHeight             = targetHeight ;
            sut.Height                   = height ;
            sut.Speed                    = speed ;
            sut.MaxSpeed                 = maxSpeed ;
            sut.MaxSpeedToStopMovement   = maxMovement ;
            sut.FudgeFactor              = fudgeFactor ;
            sut.MoveIntoDirection        = moveIntoDirection ;
            sut.StartMovingIntoDirection = startMovingIntoDirection ;

            sut.Calculate ( )
               .Delta
               .Should ( )
               .Be ( delta ) ;
        }

        [ DataRow ( 2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    0 ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    20 ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    - 20 ) ]
        [ DataTestMethod ]
        public void Calculate_ForParameters_SetsMovementUntilStop (
            uint  targetHeight ,
            uint  height ,
            int   speed ,
            int   maxSpeed ,
            uint  maxMovement ,
            float fudgeFactor ,
            int   movementUntilStop )
        {
            var sut = CreateSut ( ) ;

            sut.TargetHeight           = targetHeight ;
            sut.Height                 = height ;
            sut.Speed                  = speed ;
            sut.MaxSpeed               = maxSpeed ;
            sut.MaxSpeedToStopMovement = maxMovement ;
            sut.FudgeFactor            = fudgeFactor ;

            sut.Calculate ( )
               .MovementUntilStop
               .Should ( )
               .Be ( movementUntilStop ) ;
        }

        // Same Direction
        [ DataRow ( Direction.None ,
                    Direction.None ,
                    2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    2000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    false ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    2000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.None ,
                    Direction.None ,
                    1000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    1000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    1000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    1000u ,
                    1000u ,
                    100 ,
                    100 ,
                    20u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    1000u ,
                    1000u ,
                    100 ,
                    100 ,
                    21u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    1000u ,
                    1001u ,
                    100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    1000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    1000u ,
                    980u ,
                    100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Up ,
                    1000u ,
                    979u ,
                    100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    false ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    1000u ,
                    1021u ,
                    - 100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    false ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    1000u ,
                    1020u ,
                    - 100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    1000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Down ,
                    1000u ,
                    999u ,
                    - 100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        // Opposite Direction
        [ DataRow ( Direction.None ,
                    Direction.Up ,
                    2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    false ) ]
        [ DataRow ( Direction.None ,
                    Direction.Down ,
                    2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Down ,
                    2000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Up ,
                    2000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    false ) ]
        [ DataRow ( Direction.None ,
                    Direction.None ,
                    1000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Down ,
                    1000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Up ,
                    1000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    2.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Down ,
                    1000u ,
                    1000u ,
                    100 ,
                    100 ,
                    20u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Down ,
                    1000u ,
                    1000u ,
                    100 ,
                    100 ,
                    21u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Down ,
                    1000u ,
                    1001u ,
                    100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Down ,
                    1000u ,
                    1000u ,
                    100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Down ,
                    1000u ,
                    980u ,
                    100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Up ,
                    Direction.Down ,
                    1000u ,
                    979u ,
                    100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Up ,
                    1000u ,
                    1021u ,
                    - 100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Up ,
                    1000u ,
                    1020u ,
                    - 100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Up ,
                    1000u ,
                    1000u ,
                    - 100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataRow ( Direction.Down ,
                    Direction.Up ,
                    1000u ,
                    999u ,
                    - 100 ,
                    100 ,
                    10u ,
                    1.0f ,
                    true ) ]
        [ DataTestMethod ]
        public void Calculate_ForParameters_SetsHasReachedTargetHeight (
            Direction moveIntoDirection ,
            Direction startMovingIntoDirection ,
            uint      targetHeight ,
            uint      height ,
            int       speed ,
            int       maxSpeed ,
            uint      maxMovement ,
            float     fudgeFactor ,
            bool      hasReachedTargetHeight )
        {
            var sut = CreateSut ( ) ;

            sut.MoveIntoDirection        = moveIntoDirection ;
            sut.TargetHeight             = targetHeight ;
            sut.Height                   = height ;
            sut.Speed                    = speed ;
            sut.MaxSpeed                 = maxSpeed ;
            sut.MaxSpeedToStopMovement   = maxMovement ;
            sut.FudgeFactor              = fudgeFactor ;
            sut.StartMovingIntoDirection = startMovingIntoDirection ;

            sut.Calculate ( )
               .HasReachedTargetHeight
               .Should ( )
               .Be ( hasReachedTargetHeight ) ;
        }

        [ DataRow ( 2000u ,
                    1000u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.Up ) ]
        [ DataRow ( 2000u ,
                    1989u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.Up ) ]
        [ DataRow ( 2000u ,
                    1990u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.None ) ]
        [ DataRow ( 2000u ,
                    1999u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.None ) ]
        [ DataRow ( 2000u ,
                    2000u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.None ) ]
        [ DataRow ( 2000u ,
                    3000u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.Down ) ]
        [ DataRow ( 2000u ,
                    2011u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.Down ) ]
        [ DataRow ( 2000u ,
                    2010u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.None ) ]
        [ DataRow ( 2000u ,
                    2001u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.None ) ]
        [ DataRow ( 2000u ,
                    2000u ,
                    0 ,
                    100 ,
                    10u ,
                    1.0f ,
                    Direction.None ) ]
        [ DataRow ( 2000u ,
                    1979u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    Direction.Up ) ]
        [ DataRow ( 2000u ,
                    1980u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    Direction.None ) ]
        [ DataRow ( 2000u ,
                    2020u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    Direction.None ) ]
        [ DataRow ( 2000u ,
                    2021u ,
                    0 ,
                    100 ,
                    10u ,
                    2.0f ,
                    Direction.Down ) ]
        [ DataTestMethod ]
        public void Calculate_ForParameters_SetsMoveIntoDirection (
            uint      targetHeight ,
            uint      height ,
            int       speed ,
            int       maxSpeed ,
            uint      maxMovement ,
            float     fudgeFactor ,
            Direction moveIntoDirection )
        {
            var sut = CreateSut ( ) ;

            sut.MoveIntoDirection      = moveIntoDirection ;
            sut.TargetHeight           = targetHeight ;
            sut.Height                 = height ;
            sut.Speed                  = speed ;
            sut.MaxSpeed               = maxSpeed ;
            sut.MaxSpeedToStopMovement = maxMovement ;
            sut.FudgeFactor            = fudgeFactor ;

            sut.Calculate ( )
               .MoveIntoDirection
               .Should ( )
               .Be ( moveIntoDirection ) ;
        }

        private StoppingHeightCalculator CreateSut ( )
        {
            return new StoppingHeightCalculator ( _logger ,
                                                  _calculator ) ;
        }

        private IHasReachedTargetHeightCalculator _calculator ;
        private ILogger                           _logger ;
    }
}