﻿NOTE: This is a pre-release where all required security checks and error handling is not fully completed, although it is feature-complete for testing.

Quick start:
You can use this package to configure either the AS, WSP or both by configuring it into a OWIN pipeline.
Dependendent on your requirements the AS and WSP can either run in the same OWIN pipeline sharing internal data structures or in seperate distributed applications using RESTful communication between them.
The example below shows the simplest possible configuration using in-memory data structures

	public void Configuration(IAppBuilder app)
        {
            app.SetLoggerFactory(new YourLoggerFactory()); //Sets the OWIN logger factory which the middlewares logs to

            .UseOioIdwsAuthentication(new OioIdwsAuthenticationOptions()) //registers the middleware for the WSP that handles authentication, see options for possible configurations
            .UseOioIdwsAuthorizationService(new OioIdwsAuthorizationServiceOptions //registers the middleware for the AS
            {
                AccessTokenIssuerPath = new PathString("/accesstoken/issue"), //path where access tokens are issued
				//possible
                IssuerAudiences = () => Task.FromResult(new[]
                {
                    new IssuerAudiences("STS cert thumbprint", "certificate friendly name")
                        .Audience(new Uri("https://myaudience.dk")),
                }),
            })
            .Use<MyWspService>(); //Your WSP, if it's an OWIN middleware, or whatever else you might implement it as, e.g. web api
        }

Introduction:
Digst.OioIdws.Rest.Server is a .Net-based reference implementation made as OWIN middleware of the OIOIDWS 1.0.1a profile which is described at http://digitaliser.dk/resource/526486.
This package can be used by services to act as a Web Service Producer (WSP) and/or Authorization Server (AS).
The goal of this component is to make it easy for Web Service Providers (WSP) to support the OIO Identity-based Web Services (OIOIDWS REST) profile.

The implementation is based on the following standards for communication with a web service consumer (WSC).
[OIO-IDWS] - OIO Identity-Based Web Services 1.1: This document describes the overall business goals and requirements and shows how the different OIO profiles are combined to achieve these. Scenario 1 specifies that either WS-Security or a Liberty WSF-Profile can be used. Scenario 4 mandates LIB-BAS between WSC and WSP.

[OIO-IDT] - OIO SAML Profile for Identity Tokens 1.0: Specifies that only "Holder of key" confirmation method should be allowed.

[OIO-IDWS-REST] - Adds a profile for allowing RESTful services in the OIO IDWS specification, although [OIO-IDWS] has not been updated to reflect the added profile.

All above specifications can be found through https://test-nemlog-in.dk/Testportal/ or http://digitaliser.dk/resource/526486. They are also located in the "Misc\Specifications" folder on Softwarebørsen. It is the copies on Softwarebørsen that this implementation follows.

Requirements:
.Net 4.5 Framework.

Logging:
Logging uses OWIN standard logging interface. For configuring the OWIN logger factory see the example in Digst.OioIdws.Rest.Examples.ServerCombined

Replay attack:
TODO

Test:
TODO

Examples:
Please checkout the complete OIOIDWS.Net reference implementation at Softwarebørsen (https://svn.softwareborsen.dk/OIOIDWS/trunk). Here is a couple of example projects that shows how you can run the AS and WSP in the OWIN pipeline:
Digst.OioIdws.Rest.Examples.AS - How to configure an application to run the AS and host endpoints for issuing access tokens and delivering token information to the WSP.
Digst.OioIdws.Rest.Examples.WSP - How to configure an application to run the WSP with an OWIN authentication middelware that sits in front and authenticates the user using the AS.
Digst.OioIdws.Rest.Examples.ServerCombined - How to configure a single application to host both the AS and WSP where communication between them can be done in memory instead of through web boundaries.