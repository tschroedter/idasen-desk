using System ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests
{
    [ TestClass ]
    public class GuardTests
    {
        public static void AssertException ( Action action ,
                                             Type   type ,
                                             string parameter )
        {
            using ( new AssertionScope ( ) )
            {
                action.Should ( )
                      .Throw < Exception > ( )
                      .And.GetType ( )
                      .Should ( )
                      .Be ( type ) ;
            }
        }

        [ DataTestMethod ]
        [ DynamicData ( nameof ( GuardTestData.NullEmptyOrWhitespace ) ,
                        typeof ( GuardTestData ) ,
                        DynamicDataSourceType.Method ) ]
        public void ArgumentNotEmptyOrWhitespace_ForInvalidValues_Throws ( string value ,
                                                                           Type   type )
        {
            AssertException ( ( ) => Guard.ArgumentNotEmptyOrWhitespace ( value ,
                                                                          "parameter" ) ,
                              type ,
                              "parameter" ) ;
        }

        [ DataTestMethod ]
        [ DynamicData ( nameof ( GuardTestData.InstanceAndInteger ) ,
                        typeof ( GuardTestData ) ,
                        DynamicDataSourceType.Method ) ]
        public void ArgumentNotEmptyOrWhitespace_ForValues_DoesNotThrows ( object value )
        {
            var action = new Action ( ( ) => Guard.ArgumentNotEmptyOrWhitespace ( value ,
                                                                                  "parameter" ) ) ;

            action.Should ( )
                  .NotThrow ( ) ;
        }

        [ DataTestMethod ]
        [ DynamicData ( nameof ( GuardTestData.InstanceAndInteger ) ,
                        typeof ( GuardTestData ) ,
                        DynamicDataSourceType.Method ) ]
        public void ArgumentNotNull_ForValueNotNull_DoesNotThrows ( object value )
        {
            var action = new Action ( ( ) => Guard.ArgumentNotNull ( value ,
                                                                     "parameter" ) ) ;

            action.Should ( )
                  .NotThrow ( ) ;
        }

        [ TestMethod ]
        public void ArgumentNotNull_ForValueNull_Throws ( )
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var action = new Action ( ( ) => Guard.ArgumentNotNull ( null ,
                                                                     "parameter" ) ) ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .And.ParamName.Should ( )
                  .Be ( "parameter" ) ;
        }

        [ DataTestMethod ]
        [ DynamicData ( nameof ( GuardTestData.InstanceAndInteger ) ,
                        typeof ( GuardTestData ) ,
                        DynamicDataSourceType.Method ) ]
        public void ArgumentNotNullOrEmpty_ForValues_DoesNotThrows ( object value )
        {
            var action = new Action ( ( ) => Guard.ArgumentNotNullOrEmpty ( value ,
                                                                            "parameter" ) ) ;

            action.Should ( )
                  .NotThrow ( ) ;
        }

        [ DataTestMethod ]
        [ DynamicData ( nameof ( GuardTestData.NullOrEmpty ) ,
                        typeof ( GuardTestData ) ,
                        DynamicDataSourceType.Method ) ]
        public void ArgumentNotNullOrEmpty_ForValues_Throws ( string value ,
                                                              Type   type )
        {
            AssertException ( ( ) => Guard.ArgumentNotNullOrEmpty ( value ,
                                                                    "parameter" ) ,
                              type ,
                              "parameter" ) ;
        }
    }
}