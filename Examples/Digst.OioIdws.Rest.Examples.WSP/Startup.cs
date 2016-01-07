﻿using System;
using Digst.OioIdws.Rest.Server.Wsp;
using Microsoft.Owin.Logging;
using Owin;

namespace Digst.OioIdws.Rest.Examples.WSP
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetLoggerFactory(new ConsoleLoggerFactory());

            app
                .UseErrorPage()
                .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions
                {
                    TokenProvider = new RestTokenProvider(new Uri("https://digst.oioidws.rest.as:10001/accesstoken"))
                    
                })
                .Use<MyService>();
        }
    }
}
