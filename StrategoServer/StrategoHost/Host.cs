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
    public static class Host
    {
        static void Main(string[] args)
        {
            var logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(logConfigPath));

            var log = LogManager.GetLogger(typeof(Host));

            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<ServicesModule>();
                var container = builder.Build();
                AutofacHostFactory.Container = container;

                using (var scope = container.BeginLifetimeScope())
                {
                    var services = ResolveServices(scope);
                    var (loginHost, chatHost, profileHost, roomHost, friendHost, gameHost, pingHost) = CreateServiceHosts(services);

                    bool loginServiceOpened = OpenService(loginHost, log, "Login");
                    bool chatServiceOpened = OpenService(chatHost, log, "Chat");
                    bool profileServiceOpened = OpenService(profileHost, log, "Profile");
                    bool roomServiceOpened = OpenService(roomHost, log, "Room");
                    bool friendServiceOpened = OpenService(friendHost, log, "Friend");
                    bool gameServiceOpened = OpenService(gameHost, log, "Game");
                    bool pingServiceOpened = OpenService(pingHost, log, "Ping");

                    Console.ReadLine();

                    CloseService(loginHost, log, "Login", loginServiceOpened);
                    CloseService(chatHost, log, "Chat", chatServiceOpened);
                    CloseService(profileHost, log, "Profile", profileServiceOpened);
                    CloseService(roomHost, log, "Room", roomServiceOpened);
                    CloseService(friendHost, log, "Friend", friendServiceOpened);
                    CloseService(gameHost, log, "Game", gameServiceOpened);
                    CloseService(pingHost, log, "Ping", pingServiceOpened);
                }
            }
            catch (FileNotFoundException fex)
            {
                log.Fatal(Messages.FileNotFound, fex);
                Console.WriteLine($"{Messages.FileNotFound}: {fex.Message}");
                Console.ReadLine();
            }
            catch (UnauthorizedAccessException uex)
            {
                log.Fatal(Messages.UnauthorizedAccess, uex);
                Console.WriteLine(Messages.UnauthorizedAccess, uex);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                log.Fatal(Messages.UnexpectedError, ex);
                Console.WriteLine($"{Messages.UnexpectedError}: {ex.Message}");
                Console.ReadLine();
            }
        }

        private static (ILogInService loginService, IChatService chatService, IProfileDataService profileService, IRoomService roomService, IFriendOperationsService friendService, IGameService gameService, IPingService pingService) ResolveServices(ILifetimeScope scope)
        {
            var loginService = scope.Resolve<ILogInService>();
            var chatService = scope.Resolve<IChatService>();
            var profileService = scope.Resolve<IProfileDataService>();
            var roomService = scope.Resolve<IRoomService>();
            var friendService = scope.Resolve<IFriendOperationsService>();
            var gameService = scope.Resolve<IGameService>();
            var pingService = scope.Resolve<IPingService>();

            return (loginService, chatService, profileService, roomService, friendService, gameService, pingService);
        }

        private static (ServiceHost loginHost, ServiceHost chatHost, ServiceHost profileHost, ServiceHost roomHost, ServiceHost friendHost, ServiceHost gameHost, ServiceHost pingHost)
        CreateServiceHosts((ILogInService loginService, IChatService chatService, IProfileDataService profileService, IRoomService roomService, IFriendOperationsService friendService, IGameService gameService, IPingService pingService) services)
        {
            var loginHost = new ServiceHost(services.loginService);
            var chatHost = new ServiceHost(services.chatService);
            var profileHost = new ServiceHost(services.profileService);
            var roomHost = new ServiceHost(services.roomService);
            var friendHost = new ServiceHost(services.friendService);
            var gameHost = new ServiceHost(services.gameService);
            var pingHost = new ServiceHost(services.pingService);

            return (loginHost, chatHost, profileHost, roomHost, friendHost, gameHost, pingHost);
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
                log.Fatal($"Failed to start {serviceName} service: {ex.Message}");
                Console.WriteLine($"Failed to start {serviceName} service: {ex.Message}");
                return false;
            }
            catch (TimeoutException ex)
            {
                log.Fatal($"Failed to start {serviceName} service: {ex.Message}");
                Console.WriteLine($"Failed to start {serviceName} service: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                log.Fatal($"Failed to start {serviceName} service: {ex.Message}");
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
                    log.Error($"Failed to close {serviceName} service: {ex.Message}");
                    Console.WriteLine($"Failed to close {serviceName} service: {ex.Message}");
                }
                catch (TimeoutException ex)
                {
                    log.Error($"Failed to close {serviceName} service: {ex.Message}");
                    Console.WriteLine($"Failed to close {serviceName} service: {ex.Message}");
                }
                catch (Exception ex)
                {
                    log.Error($"Failed to close {serviceName} service: {ex.Message}");
                    Console.WriteLine($"Failed to close {serviceName} service: {ex.Message}");
                }
            }
        }
    }
}
