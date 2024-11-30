using Autofac.Integration.Wcf;
using Autofac;
using StrategoServices;
using StrategoServices.Logic;
using StrategoServices.Services;
using System;
using System.ServiceModel;
using log4net;
using StrategoServices.Services.Interfaces;
using Utilities;
using log4net.Config;
using System.IO;

namespace StrategoHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(logConfigPath));

            var log = LogManager.GetLogger(typeof(Program));

            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<ServicesModule>();
                var container = builder.Build();
                AutofacHostFactory.Container = container;

                using (var scope = container.BeginLifetimeScope())
                {
                    var services = ResolveServices(scope);
                    var hosts = CreateServiceHosts(services);

                    bool loginServiceOpened = OpenService(hosts.loginHost, log, "Login");
                    bool chatServiceOpened = OpenService(hosts.chatHost, log, "Chat");
                    bool profileServiceOpened = OpenService(hosts.profileHost, log, "Profile");
                    bool roomServiceOpened = OpenService(hosts.roomHost, log, "Room");
                    bool friendServiceOpened = OpenService(hosts.friendHost, log, "Friend");
                    bool gameServiceOpened = OpenService(hosts.gameHost, log, "Game");

                    Console.ReadLine();

                    CloseService(hosts.loginHost, log, "Login", loginServiceOpened);
                    CloseService(hosts.chatHost, log, "Chat", chatServiceOpened);
                    CloseService(hosts.profileHost, log, "Profile", profileServiceOpened);
                    CloseService(hosts.roomHost, log, "Room", roomServiceOpened);
                    CloseService(hosts.friendHost, log, "Friend", friendServiceOpened);
                    CloseService(hosts.gameHost, log, "Game", gameServiceOpened);
                }
            }
            catch (FileNotFoundException ex)
            {
                log.Fatal("File not found: " + ex.Message);
                Console.WriteLine($"File not found: {ex.Message}");
                Console.ReadLine();
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Fatal("Unauthorized access: " + ex.Message);
                Console.WriteLine($"Unauthorized access: {ex.Message}");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                log.Fatal("Unexpected error: " + ex.Message);
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.ReadLine();
            }
        }

        private static (ILogInService loginService, IChatService chatService, IProfileDataService profileService, IRoomService roomService, IFriendOperationsService friendService, IGameService gameService) ResolveServices(ILifetimeScope scope)
        {
            var loginService = scope.Resolve<ILogInService>();
            var chatService = scope.Resolve<IChatService>();
            var profileService = scope.Resolve<IProfileDataService>();
            var roomService = scope.Resolve<IRoomService>();
            var friendService = scope.Resolve<IFriendOperationsService>();
            var gameService = scope.Resolve<IGameService>();

            return (loginService, chatService, profileService, roomService, friendService, gameService);
        }

        private static (ServiceHost loginHost, ServiceHost chatHost, ServiceHost profileHost, ServiceHost roomHost, ServiceHost friendHost, ServiceHost gameHost) CreateServiceHosts((ILogInService loginService, IChatService chatService, IProfileDataService profileService, IRoomService roomService, IFriendOperationsService friendService, IGameService gameService) services)
        {
            var loginHost = new ServiceHost(services.loginService);
            var chatHost = new ServiceHost(services.chatService);
            var profileHost = new ServiceHost(services.profileService);
            var roomHost = new ServiceHost(services.roomService);
            var friendHost = new ServiceHost(services.friendService);
            var gameHost = new ServiceHost(services.gameService);

            return (loginHost, chatHost, profileHost, roomHost, friendHost, gameHost);
        }

        private static bool OpenService(ServiceHost host, ILog log, string serviceName)
        {
            try
            {
                host.Open();
                log.Info($"{serviceName} service is running...");
                Console.WriteLine($"{serviceName} service is running...");
                return true;
            }
            catch (CommunicationException ex)
            {
                log.Fatal($"Failed to start {serviceName} service: " + ex.Message);
                Console.WriteLine($"Failed to start {serviceName} service: {ex.Message}");
                return false;
            }
            catch (TimeoutException ex)
            {
                log.Fatal($"Failed to start {serviceName} service: " + ex.Message);
                Console.WriteLine($"Failed to start {serviceName} service: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                log.Fatal($"Failed to start {serviceName} service: " + ex.Message);
                Console.WriteLine($"Failed to start {serviceName} service: {ex.Message}");
                return false;
            }
        }

        private static void CloseService(ServiceHost host, ILog log, string serviceName, bool serviceOpened)
        {
            if (serviceOpened)
            {
                try
                {
                    host.Close();
                    log.Info($"{serviceName} service closed.");
                }
                catch (CommunicationException ex)
                {
                    log.Error($"Failed to close {serviceName} service: " + ex.Message);
                    Console.WriteLine($"Failed to close {serviceName} service: {ex.Message}");
                }
                catch (TimeoutException ex)
                {
                    log.Error($"Failed to close {serviceName} service: " + ex.Message);
                    Console.WriteLine($"Failed to close {serviceName} service: {ex.Message}");
                }
                catch (Exception ex)
                {
                    log.Error($"Failed to close {serviceName} service: " + ex.Message);
                    Console.WriteLine($"Failed to close {serviceName} service: {ex.Message}");
                }
            }
        }
    }
}
