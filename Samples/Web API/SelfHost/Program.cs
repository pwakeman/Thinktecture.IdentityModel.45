﻿using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Resources;
using Resources.Configuration;
using Resources.Security;
using Thinktecture.IdentityModel.Tokens.Http;
using Thinktecture.IdentityModel.WebApi;

namespace SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration(Constants.SelfHostBaseAddress);

            ConfigureApis(config);

            config.Routes.MapHttpRoute(
                "API Default",
                "{controller}/{id}",
                new { id = RouteParameter.Optional });

            var server = new HttpSelfHostServer(config);

            server.OpenAsync().Wait();

            Console.WriteLine("Server is running.");
            Console.ReadLine();

        }

        private static void ConfigureApis(HttpSelfHostConfiguration configuration)
        {
            // authentication
            var authConfig = AuthenticationConfig.CreateConfiguration();
            authConfig.ClaimsAuthenticationManager = new ConsultantsClaimsTransformer();
            configuration.MessageHandlers.Add(new AuthenticationHandler(authConfig));

            // authorization
            configuration.SetAuthorizationManager(new GlobalAuthorization(DefaultPolicy.Deny));

            // CORS
            CorsConfig.RegisterGlobal(configuration);

            // dependency resolver for authorization manager
            configuration.DependencyResolver = new AuthorizationDependencyResolver();
        }
    }
}
