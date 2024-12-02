using StrategoServices.Logic;
using StrategoServices.Services.Interfaces;
using StrategoServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using StrategoDataAccess;

namespace StrategoServices
{
    using Autofac;

    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RoomService>().As<IRoomService>().InstancePerLifetimeScope();
            builder.RegisterType<ChatService>().As<IChatService>().InstancePerLifetimeScope();
            builder.RegisterType<LogInService>().As<ILogInService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileService>().As<IProfileDataService>().InstancePerLifetimeScope();
            builder.RegisterType<FriendOperationsService>().As<IFriendOperationsService>().InstancePerLifetimeScope();
            builder.RegisterType<GameService>().As<IGameService>().InstancePerLifetimeScope();

            builder.RegisterType<AccountManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ProfilesManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<FriendsManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PasswordManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InvitationManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<WinsManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ReportPlayerManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ConnectedPlayersManager>().AsSelf().SingleInstance();

            builder.RegisterType<AccountRepository>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PlayerRepository>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PictureRepository>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<LabelRepository>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<GamesRepository>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<FriendsRepository>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<StrategoEntities>().AsSelf().InstancePerDependency();
        }
    }

}
