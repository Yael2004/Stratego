using Autofac.Integration.Wcf;
using Autofac;
using StrategoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using StrategoCore.Services;

namespace StrategoDataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<StrategoEntities>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AccountRepository>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AccountManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<LogInService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ChatService>().AsSelf().InstancePerLifetimeScope();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var loginService = scope.Resolve<LogInService>();
                var chatService = scope.Resolve<ChatService>();

                using (var loginHost = new ServiceHost(loginService))
                using (var chatHost = new ServiceHost(chatService))
                {
                    loginHost.Open();
                    chatHost.Open();

                    Console.WriteLine("Stratego server is running with both services...");
                    Console.ReadLine();
                }
            }
        }

    }
}
