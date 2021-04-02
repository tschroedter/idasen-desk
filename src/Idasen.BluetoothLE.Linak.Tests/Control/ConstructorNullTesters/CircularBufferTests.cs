using System ;
using FluentAssertions ;
using FluentAssertions.Common ;
using FluentAssertions.Execution ;
using Idasen.BluetoothLE.Linak.Control ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Linak.Tests.Control.ConstructorNullTesters
{
    /// <summary>
    ///     The original source code from https://github.com/joaoportela/CircularBuffer-CSharp.
    ///     was modified to support MSTest and FluentAssertions.
    /// </summary>
    [ TestClass ]
    public class CircularBufferTests
    {
        [ TestMethod ]
        public void CircularBuffer_GetEnumeratorConstructorCapacity_ReturnsEmptyCollection ( )
        {
            var buffer = new CircularBuffer < string > ( 5 ) ;

            buffer.Should ( )
                  .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_ConstructorSizeIndexAccess_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 } ) ;

            using var scope = new AssertionScope ( ) ;

            buffer.Capacity
                  .Should ( )
                  .Be ( 5 ) ;
            buffer.Size
                  .Should ( )
                  .Be ( 4 ) ;

            for ( var i = 0 ; i < 4 ; i ++ )
            {
                buffer [ i ].Should ( )
                            .Be ( i ) ;
            }
        }

        [ TestMethod ]
        public void CircularBuffer_Constructor_ExceptionWhenSourceIsLargerThanCapacity ( )
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = ( ) => new CircularBuffer < int > ( 3 ,
                                                                new [ ] { 0 , 1 , 2 , 3 } ) ;

            action.Should ( )
                  .Throw < ArgumentException > ( )
                  .WithMessage ( "Too many items to fit circular buffer (Parameter 'items')" ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_GetEnumeratorConstructorDefinedArray_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 } ) ;

            using var scope = new AssertionScope ( ) ;

            var x = 0 ;
            foreach ( var item in buffer )
            {
                item.Should ( )
                    .Be ( x ) ;
                x ++ ;
            }
        }

        [ TestMethod ]
        public void CircularBuffer_PushBack_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ) ;

            for ( var i = 0 ; i < 5 ; i ++ )
            {
                buffer.PushBack ( i ) ;
            }

            using var scope = new AssertionScope ( ) ;

            buffer.Front ( )
                  .Should ( )
                  .Be ( 0 ) ;

            for ( var i = 0 ; i < 5 ; i ++ )
            {
                buffer [ i ].Should ( )
                            .Be ( i ) ;
            }
        }

        [ TestMethod ]
        public void CircularBuffer_PushBackOverflowingBuffer_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ) ;

            for ( var i = 0 ; i < 10 ; i ++ )
            {
                buffer.PushBack ( i ) ;
            }

            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 5 , 6 , 7 , 8 , 9 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_GetEnumeratorOverflowedArray_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ) ;

            for ( var i = 0 ; i < 10 ; i ++ )
            {
                buffer.PushBack ( i ) ;
            }

            using var scope = new AssertionScope ( ) ;

            // buffer should have [5,6,7,8,9]
            var x = 5 ;
            foreach ( var item in buffer )
            {
                item.Should ( )
                    .IsSameOrEqualTo ( x ) ;
                x ++ ;
            }
        }

        [ TestMethod ]
        public void CircularBuffer_ToArrayConstructorDefinedArray_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 } ) ;

            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 0 , 1 , 2 , 3 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_ToArrayOverflowedBuffer_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ) ;

            for ( var i = 0 ; i < 10 ; i ++ )
            {
                buffer.PushBack ( i ) ;
            }

            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 5 , 6 , 7 , 8 , 9 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_PushFront_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ) ;

            for ( var i = 0 ; i < 5 ; i ++ )
            {
                buffer.PushFront ( i ) ;
            }

            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 4 , 3 , 2 , 1 , 0 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_PushFrontAndOverflow_CorrectContent ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ) ;

            for ( var i = 0 ; i < 10 ; i ++ )
            {
                buffer.PushFront ( i ) ;
            }

            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 9 , 8 , 7 , 6 , 5 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_Front_CorrectItem ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) ;

            buffer.Front ( )
                  .Should ( )
                  .Be ( 0 ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_Back_CorrectItem ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) ;

            buffer.Back ( )
                  .Should ( )
                  .Be ( 4 ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_BackOfBufferOverflowByOne_CorrectItem ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) ;
            buffer.PushBack ( 42 ) ;

            using var scope = new AssertionScope ( ) ;

            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 1 , 2 , 3 , 4 , 42 } ) ;
            buffer.Back ( )
                  .Should ( )
                  .Be ( 42 ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_Front_EmptyBufferThrowsException ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ) ;

            Action action = ( ) => buffer.Front ( ) ;

            action.Should ( )
                  .Throw < InvalidOperationException > ( )
                  .WithMessage ( "Cannot access an empty buffer." ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_Back_EmptyBufferThrowsException ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ) ;

            Action action = ( ) => buffer.Back ( ) ;

            action.Should ( )
                  .Throw < InvalidOperationException > ( )
                  .WithMessage ( "Cannot access an empty buffer." ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_PopBack_RemovesBackElement ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) ;

            buffer.Size
                  .Should ( )
                  .Be ( 5 ) ;

            buffer.PopBack ( ) ;

            using var scope = new AssertionScope ( ) ;

            buffer.Size
                  .Should ( )
                  .Be ( 4 ) ;
            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 0 , 1 , 2 , 3 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_PopBackInOverflowBuffer_RemovesBackElement ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) ;
            buffer.PushBack ( 5 ) ;

            using var scope = new AssertionScope ( ) ;

            buffer.Size
                  .Should ( )
                  .Be ( 5 ) ;
            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 1 , 2 , 3 , 4 , 5 } ) ;

            buffer.PopBack ( ) ;

            buffer.Size
                  .Should ( )
                  .Be ( 4 ) ;
            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 1 , 2 , 3 , 4 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_PopFront_RemovesBackElement ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) ;

            using var scope = new AssertionScope ( ) ;

            buffer.Size
                  .Should ( )
                  .Be ( 5 ) ;

            buffer.PopFront ( ) ;

            buffer.Size
                  .Should ( )
                  .Be ( 4 ) ;
            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 1 , 2 , 3 , 4 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_PopFrontInOverflowBuffer_RemovesBackElement ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) ;
            buffer.PushFront ( 5 ) ;

            using var scope = new AssertionScope ( ) ;

            buffer.Size
                  .Should ( )
                  .Be ( 5 ) ;
            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 5 , 0 , 1 , 2 , 3 } ) ;

            buffer.PopFront ( ) ;

            buffer.Size
                  .Should ( )
                  .Be ( 4 ) ;
            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 0 , 1 , 2 , 3 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_SetIndex_ReplacesElement ( )
        {
            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) { [ 1 ] = 10 , [ 3 ] = 30 } ;


            buffer.ToArray ( )
                  .Should ( )
                  .BeEquivalentTo ( new [ ] { 0 , 10 , 2 , 30 , 4 } ) ;
        }

        [ TestMethod ]
        public void CircularBuffer_WithDifferentSizeAndCapacity_BackReturnsLastArrayPosition ( )
        {
            // test to confirm this issue does not happen anymore:
            // https://github.com/joaoportela/CircularBuffer-CSharp/issues/2

            var buffer = new CircularBuffer < int > ( 5 ,
                                                      new [ ] { 0 , 1 , 2 , 3 , 4 } ) ;

            buffer.PopFront ( ) ; // (make size and capacity different)

            buffer.Back ( )
                  .Should ( )
                  .Be ( 4 ) ;
        }
    }
}