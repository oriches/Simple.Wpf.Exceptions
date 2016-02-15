namespace Simple.Wpf.Exceptions.Extensions
{
    using System;
    using Services;

    public static class BaseServiceExtensions
    {
        public static T DisposeWith<T>(this T instance, BaseService service) where T : IDisposable
        {
            service.Add(instance);

            return instance;
        }
    }
}