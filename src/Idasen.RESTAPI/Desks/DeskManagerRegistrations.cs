﻿using System.Threading.Tasks ;
using Autofac ;
using Idasen.Launcher ;
using Idasen.RESTAPI.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.RESTAPI.Desks
{
    public static class DeskManagerRegistrations
    {
        [ UsedImplicitly ] public static Task < bool > DeskManager ;

        public static IDeskManager CreateFakeDeskManager ( )
        {
            return new FakeDeskManager ( ) ;
        }

        public static IDeskManager CreateRealDeskManager ( )
        {
            ContainerProvider.Builder.RegisterModule ( new IdasenRESTAPIModule ( ) ) ;

            var container = ContainerProvider.Create ( "Idasen.ConsoleDapr" ,
                                                       "Idasen.ConsoleDapr.log" ) ;

            var manager = container.Resolve < IDeskManager > ( ) ;

            while ( ! ( DeskManager is { Result: true } ) )
            {
                DeskManager = Task.Run ( async ( ) => await manager.Initialise ( )
                                                                   .ConfigureAwait ( false ) ) ;
            }

            return manager ;
        }
    }
}