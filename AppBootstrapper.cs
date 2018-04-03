using System;
using Stylet;
using StyletIoC;
using DotaHackLoader.ViewModels;
using System.Windows;
using System.Windows.Threading;
using System.IO;

namespace DotaHackLoader
{
    class AppBootstrapper : Bootstrapper<MainViewModel>
    {
        protected override void OnStart()
        {
         //   This is called just after the application is started, but before the IoC container is set up.
        	//Set up things like logging, etc
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
           // Bind your own types. Concrete types are automatically self - bound.
           builder.Bind(typeof(ITabItem)).ToAllImplementations();
        }

        protected override void Configure()
        {
            // This is called after Stylet has created the IoC container, so this.Container exists, but before the
            // Root ViewModel is launched.
            // Configure your services, etc, in here
        }

        protected override void OnLaunch()
        {
            // This is called just after the root ViewModel has been launched
            // Something like a version check that displays a dialog might be launched from here
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (!string.IsNullOrEmpty(Services.HttpService.dotaPath) && File.Exists($"{Services.HttpService.dotaPath}\\gameinfo.gi"))
            {
                var gameinfoDefaultString = File.ReadAllText($"{Services.HttpService.dotaPath}\\gameinfo.gi");
                if (gameinfoDefaultString.Contains("\t\t\tGame\t\t\t\tdotameat\r\n"))
                {
                    
                    var position = gameinfoDefaultString.IndexOf("\t\t\tGame\t\t\t\tdotameat\r\n");
                    var defaultGameInfoString = gameinfoDefaultString.Remove(position, 21);
                    File.WriteAllText($"{Services.HttpService.dotaPath}\\gameinfo.gi", defaultGameInfoString);
                }                
            }

            // Called on Application.Exit
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            // Called on Application.DispatcherUnhandledException
        }
    }
}
