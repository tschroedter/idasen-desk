using System ;
using System.Collections.Generic ;

namespace Idasen.BluetoothLE.Core.Tests
{
    public static class GuardTestData
    {
        public const string Empty      = "" ;
        public const string Whitespace = " " ;

        private const int Integer = 1 ;

        private static readonly object Instance = new object ( ) ;


        public static IEnumerable < object [ ] > InstanceAndInteger ( )
        {
            yield return new [ ]
                         {
                             Instance
                         } ;
            yield return new object [ ]
                         {
                             Integer
                         } ;
        }

        public static IEnumerable < object [ ] > NullEmptyOrWhitespace ( )
        {
            yield return new object [ ]
                         {
                             null ,
                             typeof ( ArgumentNullException )
                         } ;
            yield return new object [ ]
                         {
                             Empty ,
                             typeof ( ArgumentException )
                         } ;
            yield return new object [ ]
                         {
                             Whitespace ,
                             typeof ( ArgumentException )
                         } ;
        }

        public static IEnumerable < object [ ] > NullOrEmpty ( )
        {
            yield return new object [ ]
                         {
                             null ,
                             typeof ( ArgumentNullException )
                         } ;
            yield return new object [ ]
                         {
                             Empty ,
                             typeof ( ArgumentException )
                         } ;
        }
    }
}