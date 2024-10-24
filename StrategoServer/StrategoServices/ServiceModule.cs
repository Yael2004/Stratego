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

namespace StrategoServices
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AccountManager>().As<AccountManager>();
            builder.RegisterType<ProfilesManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AccountManager>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
