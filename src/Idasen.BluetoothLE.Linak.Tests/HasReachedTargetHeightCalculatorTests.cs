using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Control ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ TestClass ]
    public class HasReachedTargetHeightCalculatorTests
    {
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.None ,
                    Direction.Up ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.None ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.Down ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.Down ,
                    Direction.Up ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.Down ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.Up ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    false ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.Up ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    false ) ]
        [ DataRow ( 2000u ,
                    1989u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    false ) ]
        [ DataRow ( 2000u ,
                    1990u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1991u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1999u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2000u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2001u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    true ) ]
        [ DataRow ( 2000u ,
                    3000u ,
                    10 ,
                    Direction.Up ,
                    Direction.Up ,
                    true ) ]
        [ DataRow ( 2000u ,
                    3000u ,
                    - 10 ,
                    Direction.Down ,
                    Direction.Down ,
                    false ) ]
        [ DataRow ( 2000u ,
                    2011u ,
                    - 10 ,
                    Direction.Down ,
                    Direction.Down ,
                    false ) ]
        [ DataRow ( 2000u ,
                    2010u ,
                    - 10 ,
                    Direction.Down ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2009u ,
                    - 10 ,
                    Direction.Down ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2001u ,
                    - 10 ,
                    Direction.Down ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2000u ,
                    - 10 ,
                    Direction.Down ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1999u ,
                    - 10 ,
                    Direction.Down ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    - 10 ,
                    Direction.Down ,
                    Direction.Down ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1989u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1990u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1991u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1999u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2000u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2001u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    3000u ,
                    10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1000u ,
                    - 10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1989u ,
                    - 10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1990u ,
                    - 10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1991u ,
                    - 10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    1999u ,
                    - 10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2000u ,
                    - 10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    2001u ,
                    - 10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataRow ( 2000u ,
                    3000u ,
                    - 10 ,
                    Direction.None ,
                    Direction.None ,
                    true ) ]
        [ DataTestMethod ]
        public void Calculate_ForParameters_SetsHasReachedTargetHeight (
            uint      targetHeight ,
            uint      stoppingHeight ,
            int       movementUntilStop ,
            Direction moveIntoDirection ,
            Direction startMovingIntoDirection ,
            bool      hasReachedTargetHeight )
        {
            var sut = CreateSut ( ) ;

            sut.TargetHeight             = targetHeight ;
            sut.StoppingHeight           = stoppingHeight ;
            sut.MovementUntilStop        = movementUntilStop ;
            sut.MoveIntoDirection        = moveIntoDirection ;
            sut.StartMovingIntoDirection = startMovingIntoDirection ;

            sut.Calculate ( )
               .HasReachedTargetHeight
               .Should ( )
               .Be ( hasReachedTargetHeight ) ;
        }

        [ DataRow ( 2000u ,
                    2000u ,
                    - 10 ,
                    Direction.None ,
                    0u ) ]
        [ DataRow ( 2000u ,
                    3000u ,
                    10 ,
                    Direction.None ,
                    1000u ) ]
        [ DataRow ( 2000u ,
                    3000u ,
                    - 10 ,
                    Direction.None ,
                    1000u ) ]
        [ DataRow ( 3000u ,
                    2000u ,
                    10 ,
                    Direction.None ,
                    1000u ) ]
        [ DataRow ( 3000u ,
                    2000u ,
                    - 10 ,
                    Direction.None ,
                    1000u ) ]
        [ DataTestMethod ]
        public void Calculate_ForParameters_SetsDelta (
            uint      targetHeight ,
            uint      stoppingHeight ,
            int       movementUntilStop ,
            Direction moveIntoDirection ,
            uint      delta )
        {
            var sut = CreateSut ( ) ;

            sut.TargetHeight      = targetHeight ;
            sut.StoppingHeight    = stoppingHeight ;
            sut.MovementUntilStop = movementUntilStop ;
            sut.MoveIntoDirection = moveIntoDirection ;

            sut.Calculate ( )
               .Delta
               .Should ( )
               .Be ( delta ) ;
        }

        [ TestInitialize ]
        public void Initialize ( )
        {
            _logger = Substitute.For < ILogger > ( ) ;
        }

        private HasReachedTargetHeightCalculator CreateSut ( )
        {
            return new HasReachedTargetHeightCalculator ( _logger ) ;
        }

        private ILogger _logger ;
    }
}