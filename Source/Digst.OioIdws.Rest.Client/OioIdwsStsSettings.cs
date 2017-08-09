﻿using System;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.OioWsTrust;

namespace Digst.OioIdws.Rest.Client
{
    public class OioIdwsStsSettings
    {
        public OioIdwsStsSettings()
        {
            UseTokenCache = true;
        }

        /// <summary>
        /// Represents the SecurityTokenService certificate containing only the public key. This should be a FOCES certificate.
        /// </summary>
        public X509Certificate2 Certificate { get; set; }
        /// <summary>
        /// Endpoint address of SecurityTokenService. E.g. https://SecurityTokenService.test-nemlog-in.dk/SecurityTokenService.svc
        /// </summary>
        public Uri EndpointAddress { get; set; }
        /// <summary>
        /// Allows you to increase the timeout on requests towards the SecurityTokenService. Intended for debugging purposes
        /// </summary>
        public TimeSpan? SendTimeout { get; set; }
        /// <summary>
        /// Default life time is chossen by STS if nothing is specified (8 hours according to the specification at the time of this writing).
        /// If specified, according to specification the STS is not obligated to honor this range and may return a token with a shorter life time in RSTR.
        /// All values above 480 minutes (8 hours) will result in a token life time of 8 hours.
        /// </summary>
        public TimeSpan? TokenLifeTime { get; set; }

        /// <summary>
        /// <see cref="TokenServiceConfiguration.CacheClockSkew"/>
        /// </summary>
        public TimeSpan? CacheClockSkew { get; set; }


        /// <summary>
        /// Specifies wheter or not to use the token cache variant <see cref="TokenServiceCache"/> or <see cref="TokenService"/>.
        /// If true <see cref="TokenServiceCache"/> is used which is the default.
        /// </summary>
        public bool UseTokenCache { get; set; }
    }
}