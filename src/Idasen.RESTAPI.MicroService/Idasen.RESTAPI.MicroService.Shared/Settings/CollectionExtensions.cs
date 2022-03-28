namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public static class CollectionExtensions
{
    public static string ToCsv < T > ( this ICollection < T > collection )
    {
        if ( collection.Count == 0 )
            return "[]" ;

        var items = collection.Select ( x => x?.ToString ( ) ) ;

        return string.Join ( "," ,
                             items ) ;
    }
}