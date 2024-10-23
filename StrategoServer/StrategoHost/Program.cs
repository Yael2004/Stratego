﻿using Autofac.Integration.Wcf;
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
                log.Fatal("Starting Stratego server...");
                var builder = new ContainerBuilder();

                builder.RegisterType<StrategoEntities>().AsSelf().InstancePerLifetimeScope();

                builder.RegisterType<AccountRepository>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<PlayerRepository>().AsSelf().InstancePerLifetimeScope();  
                builder.RegisterType<PictureRepository>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<LabelRepository>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<AccountManager>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<LogInService>().AsSelf().InstancePerLifetimeScope();
                builder.RegisterType<ChatService>().AsSelf().InstancePerLifetimeScope();

                var container = builder.Build();

                AutofacHostFactory.Container = container;

                using (var scope = container.BeginLifetimeScope())
                {
                    var loginService = scope.Resolve<LogInService>();
                    var chatService = scope.Resolve<ChatService>();

                    using (var loginHost = new ServiceHost(loginService))
                    using (var chatHost = new ServiceHost(chatService))
                    {
                        loginHost.Open();
                        chatHost.Open();

                        Console.WriteLine("Stratego server is running...");
                        Console.ReadLine();  
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Console.ReadLine();  
            }
        }
    }
}
