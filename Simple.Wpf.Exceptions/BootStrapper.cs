using System;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Simple.Wpf.Exceptions.ViewModels;

namespace Simple.Wpf.Exceptions
{
    public static class BootStrapper
    {
        private static ILifetimeScope _rootScope;
        private static ChromeViewModel _chromeViewModel;

        public static BaseViewModel RootVisual
        {
            get
            {
                if (_rootScope == null)
                {
                    Start();
                }

                _chromeViewModel = _rootScope.Resolve<ChromeViewModel>();
                return _chromeViewModel;
            }
        }

        public static void Start()
        {
            if (_rootScope != null)
            {
                return;
            }

            var builder = new ContainerBuilder();
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .SingleInstance()
                .AsImplementedInterfaces();
            
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"));
            
            _rootScope = builder.Build();
        }

        public static void Stop()
        {
            _rootScope.Dispose();
        }

        public static T Resolve<T>()
        {
            if (_rootScope == null)
            {
                throw new Exception("Bootstrapper hasn't been started!");
            }

            return _rootScope.Resolve<T>(new Parameter[0]);
        }

        public static T Resolve<T>(Parameter[] parameters)
        {
            if (_rootScope == null)
            {
                throw new Exception("Bootstrapper hasn't been started!");
            }

            return _rootScope.Resolve<T>(parameters);
        }
    }
}
