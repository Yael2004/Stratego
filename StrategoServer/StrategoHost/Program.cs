using Autofac.Integration.Wcf;
using Autofac;
using StrategoServices;
using StrategoServices.Logic;
using StrategoServices.Services;
using System;
using System.ServiceModel;
using log4net;
using StrategoHost.Helpers;
using StrategoServices.Services.Interfaces;

namespace StrategoHost
{
    class Program
    {
        private static readonly ILog log = Log<Program>.GetLogger();
        static void Main(string[] args)
        {
            try
            {
                var builder = new ContainerBuilder();

                builder.RegisterModule<ServicesModule>();

                var container = builder.Build();

                AutofacHostFactory.Container = container;

                using (var scope = container.BeginLifetimeScope())
                {
                    var loginService = scope.Resolve<ILogInService>();
                    var chatService = scope.Resolve<IChatService>();
                    //var profileService = scope.Resolve<IProfileService>();

                    using (var loginHost = new ServiceHost(loginService))
                    using (var chatHost = new ServiceHost(chatService))
                    //using (var profileHost = new ServiceHost(profileService))
                    {
                        loginHost.Open();
                        chatHost.Open();
                        //profileHost.Open();

                        log.Info("Stratego services are running...");

                        Console.WriteLine("Stratego services are running...");
                        Console.ReadLine();
                    }
                }
            }
            catch (CommunicationException ce)
            {
                log.Error("Communication error: " + ce.Message);
                Console.WriteLine($"Communication error: {ce.Message}");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error: " + ex.Message);
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.ReadLine();
            }
        }
    }
}
