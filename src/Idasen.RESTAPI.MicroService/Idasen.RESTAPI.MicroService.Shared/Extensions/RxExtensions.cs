using System.Diagnostics.CodeAnalysis ;
using System.Reactive ;
using System.Reactive.Linq ;
using JetBrains.Annotations ;

namespace Idasen.RESTAPI.MicroService.Shared.Extensions ;

[ ExcludeFromCodeCoverage ]
public static class RxExtensions
{
    [ UsedImplicitly ]
    public static IDisposable SubscribeAsync < T > ( this IObservable < T > source ,
                                                     Func < Task >          asyncAction ,
                                                     Action < Exception >   handler = null! )
    {
        async Task < Unit > Wrapped ( T t )
        {
            await asyncAction ( ) ;
            return Unit.Default ;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        return handler == null
                   ? source.SelectMany ( Wrapped ).Subscribe ( _ => { } )
                   : source.SelectMany ( Wrapped ).Subscribe ( _ => { } ,
                                                               handler ) ;
    }

    [ UsedImplicitly ]
    public static IDisposable SubscribeAsync < T > ( this IObservable < T > source ,
                                                     Func < T , Task >      asyncAction ,
                                                     Action < Exception >   handler = null! )
    {
        async Task < Unit > Wrapped ( T t )
        {
            await asyncAction ( t ) ;
            return Unit.Default ;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        return handler == null
                   ? source.SelectMany ( Wrapped ).Subscribe ( _ => { } )
                   : source.SelectMany ( Wrapped ).Subscribe ( _ => { } ,
                                                               handler ) ;
    }
}