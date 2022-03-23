using System.Globalization ;
using System.Runtime.CompilerServices ;
using JetBrains.Annotations ;

namespace Idasen.RESTAPI.MicroService.Shared.Extensions ;

public static class Guard
{
    private const string ValueCannotBeNullOrEmpty = "Value cannot be null or empty" ;
    private const string ValueCannotBeWhitespace  = "Value cannot be null, empty or whitespace" ;

    [ UsedImplicitly ]
    [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
    public static void ArgumentNotEmptyOrWhitespace ( object    parameter ,
                                                      string parameterName )
    {
        ArgumentNotNullOrEmpty ( parameter ,
                                 parameterName ) ;

        if ( ! ( parameter is string text ) ||
             ! string.IsNullOrWhiteSpace ( text ) )
            return ;

        var message = string.Format ( CultureInfo.InvariantCulture ,
                                      ValueCannotBeWhitespace ) ;

        throw new ArgumentException ( message ,
                                      parameterName ) ;
    }

    [ UsedImplicitly ]
    [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
    public static void ArgumentNotNull ( object    parameter ,
                                         string parameterName )
    {
        if ( parameter == null )
            throw new ArgumentNullException ( parameterName ) ;
    }

    [ UsedImplicitly ]
    [ MethodImpl ( MethodImplOptions.AggressiveInlining ) ]
    public static void ArgumentNotNullOrEmpty ( object    parameter ,
                                                string parameterName )
    {
        ArgumentNotNull ( parameter ,
                          parameterName ) ;

        if ( ! ( parameter is string text ) ||
             text.Length != 0 )
            return ;

        var message = string.Format ( CultureInfo.InvariantCulture ,
                                      ValueCannotBeNullOrEmpty ) ;

        throw new ArgumentException ( message ,
                                      parameterName ) ;
    }
}