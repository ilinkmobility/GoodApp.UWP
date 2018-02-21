using GoodApp.Callback;
using GoodApp.Views;
using Microsoft.Practices.Unity;
using Prism.Unity.Windows;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GoodApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.TryResolve<Shell>();
            shell.SetContentFrame(rootFrame);
            return shell;
        }

        /// <summary>
        /// Logic of app initialization.
        /// This is the best place to register the services in Unity container.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            Container.RegisterInstance<IDialogHelper>(new BasePage());

            Container.RegisterInstance(SessionStateService);

            Container.RegisterInstance(NavigationService);

            return base.OnInitializeAsync(args);
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("Login", null);

            return Task.FromResult(true); //This is a little trick because this method returns a Task.
        }
    }
}