using Autofac.Integration.Wcf;
using Autofac;
using StrategoDataAccess;
using StrategoServices;
using StrategoServices.Logic;
using StrategoServices.Services;
using System;
using System.ServiceModel;
using log4net;
using StrategoHost.Helpers;

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

                builder.RegisterType<StrategoEntities>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<AccountRepository>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<PlayerRepository>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<PictureRepository>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<GamesRepository>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<LabelRepository>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<ProfilesManager>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<AccountManager>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<LogInService>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<ChatService>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<ProfileService>().AsSelf().InstancePerLifetimeScope();

                var container = builder.Build();

                AutofacHostFactory.Container = container;

                using (var scope = container.BeginLifetimeScope())
                {
                    var loginService = scope.Resolve<LogInService>();
                    var chatService = scope.Resolve<ChatService>();
                    var profileService = scope.Resolve<ProfileService>();

                    using (var loginHost = new ServiceHost(loginService))
                    using (var chatHost = new ServiceHost(chatService))
                    using (var profileHost = new ServiceHost(profileService))
                    {
                       
                        loginHost.Open();
                        chatHost.Open();
                        profileHost.Open();

                        Console.WriteLine("Stratego server is running...");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
            
        }
    }
}
