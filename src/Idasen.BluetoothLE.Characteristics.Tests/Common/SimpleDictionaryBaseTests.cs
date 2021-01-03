using System ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common
{
    [ AutoDataTestClass ]
    public class SimpleDictionaryBaseTests
    {
        [ AutoDataTestMethod ]
        public void Indexer_ForNewKeyAndValue_AddsKeyAndValue (
            TestSimpleDictionaryBase sut ,
            string                   key ,
            Guid                     guid )
        {
            sut [ key ] = guid ;

            sut [ key ]
               .Should ( )
               .Be ( guid ) ;
        }

        [ AutoDataTestMethod ]
        public void Clear_ForInvoked_RemovesAllKeys (
            TestSimpleDictionaryBase sut ,
            string                   key ,
            Guid                     guid )
        {
            sut [ key ] = guid ;

            sut.Clear ( ) ;

            sut.Count
               .Should ( )
               .Be ( 0 ) ;
        }

        [ AutoDataTestMethod ]
        public void Count_ForInvoked_ReturnsCount (
            TestSimpleDictionaryBase sut ,
            string                   key ,
            Guid                     guid )
        {
            sut [ key ] = guid ;

            sut.Count
               .Should ( )
               .Be ( 1 ) ;
        }

        [ AutoDataTestMethod ]
        public void Keys_ForInvoked_ReturnsKeys (
            TestSimpleDictionaryBase sut ,
            string                   key1 ,
            Guid                     guid1 ,
            string                   key2 ,
            Guid                     guid2 )
        {
            sut [ key1 ] = guid1 ;
            sut [ key2 ] = guid2 ;

            sut.Keys
               .Should ( )
               .BeEquivalentTo ( key1 ,
                                 key2 ) ;
        }

        [ AutoDataTestMethod ]
        public void ReadOnlyDictionary_ForInvoked_ReturnsClone (
            TestSimpleDictionaryBase sut ,
            string                   key ,
            Guid                     guid ,
            Guid                     newGuid )
        {
            sut [ key ] = guid ;

            var actual = sut.ReadOnlyDictionary ;

            using var scope = new AssertionScope ( ) ;

            // modify original dictionary
            sut [ key ] = newGuid ;

            // check original dictionary
            sut [ key ]
               .Should ( )
               .Be ( newGuid ) ;

            // check cloned dictionary
            actual [ key ]
               .Should ( )
               .Be ( guid ) ;
        }
    }
}