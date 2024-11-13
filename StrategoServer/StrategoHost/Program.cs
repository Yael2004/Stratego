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
                    var profileService = scope.Resolve<IProfileDataService>();
                    var roomService = scope.Resolve<IRoomService>();
                    var friendService = scope.Resolve<IFriendOperationsService>();
                    var gameService = scope.Resolve<IGameService>();

                    var loginHost = new ServiceHost(loginService);
                    var chatHost = new ServiceHost(chatService);
                    var profileHost = new ServiceHost(profileService);
                    var roomHost = new ServiceHost(roomService);
                    var friendHost = new ServiceHost(friendService);
                    var gameHost = new ServiceHost(gameService);

                    bool loginServiceOpened = false;
                    bool chatServiceOpened = false;
                    bool profileServiceOpened = false;
                    bool roomServiceOpened = false;
                    bool friendServiceOpened = false;
                    bool gameServiceOpened = false;

                    try
                    {
                        loginHost.Open();
                        loginServiceOpened = true;
                        log.Info("Login service is running...");
                        Console.WriteLine("Login service is running...");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to start Login service: " + ex.Message);
                        Console.WriteLine($"Failed to start Login service: {ex.Message}");
                    }

                    try
                    {
                        chatHost.Open();
                        chatServiceOpened = true;
                        log.Info("Chat service is running...");
                        Console.WriteLine("Chat service is running...");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to start Chat service: " + ex.Message);
                        Console.WriteLine($"Failed to start Chat service: {ex.Message}");
                    }

                    try
                    {
                        profileHost.Open();
                        profileServiceOpened = true;
                        log.Info("Profile service is running...");
                        Console.WriteLine("Profile service is running...");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to start Profile service: " + ex.Message);
                        Console.WriteLine($"Failed to start Profile service: {ex.Message}");
                    }

                    try
                    {
                        roomHost.Open();
                        roomServiceOpened = true;
                        log.Info("Room service is running...");
                        Console.WriteLine("Room service is running...");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to start Room service: " + ex.Message);
                        Console.WriteLine($"Failed to start Room service: {ex.Message}");
                    }

                    try
                    {
                        friendHost.Open();
                        friendServiceOpened = true;
                        log.Info("Friend service is running...");
                        Console.WriteLine("Friend service is running...");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to start Friend service: " + ex.Message);
                        Console.WriteLine($"Failed to start Friend service: {ex.Message}");
                    }

                    try
                    {
                        gameHost.Open();
                        gameServiceOpened = true;
                        log.Info("Game service is running...");
                        Console.WriteLine("Game service is running...");
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to start Game service: " + ex.Message);
                        Console.WriteLine($"Failed to start Game service: {ex.Message}");
                    }

                    Console.ReadLine();

                    if (loginServiceOpened)
                    {
                        loginHost.Close();
                        log.Info("Login service closed.");
                    }

                    if (chatServiceOpened)
                    {
                        chatHost.Close();
                        log.Info("Chat service closed.");
                    }

                    if (profileServiceOpened)
                    {
                        profileHost.Close();
                        log.Info("Profile service closed.");
                    }

                    if (roomServiceOpened)
                    {
                        roomHost.Close();
                        log.Info("Room service closed.");
                    }

                    if (friendServiceOpened)
                    {
                        friendHost.Close();
                        log.Info("Friend service closed.");
                    }

                    if (gameServiceOpened)
                    {
                        gameHost.Close();
                        log.Info("Game service closed.");
                    }
                }
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
