namespace Simple.Wpf.Exceptions.Extensions
{
    using System;
    using ViewModels;

    public static class BaseViewModelExtensions
    {
        public static T DisposeWith<T>(this T instance, BaseViewModel viewModel) where T : IDisposable
        {
            viewModel.Add(instance);

            return instance;
        }
    }
}