using Velib.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Core;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

// Pour en savoir plus sur le modèle Application Hub, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace Velib
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        /// <summary>
        /// Initialise l'objet d'application de singleton.  Il s'agit de la première ligne du code créé
        /// à être exécutée. Elle correspond donc à l'équivalent logique de main() ou WinMain().
        /// </summary>
        public App()
        {   
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            
            //this.DebugSettings.EnableRedrawRegions = true;
            this.Resuming += App_Resuming;
            this.UnhandledException += App_UnhandledException;
        }

        async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var dialog = new MessageDialog(e.Message + e.Exception.InnerException + e.Exception.StackTrace);

            localSettings.Values["CrashLog"] = localSettings.Values["CrashLog"] + e.Message + e.Exception.InnerException + e.Exception.StackTrace; 
            
            await dialog.ShowAsync();
            e.Handled = true;
        }




        private async void RestoreStatus(ApplicationExecutionState previousExecutionState)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (previousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }
        }

        private Frame CreateRootFrame()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoqué lorsque l'application est lancée normalement par l'utilisateur final.  D'autres points d'entrée
        /// sont utilisés lorsque l'application est lancée pour ouvrir un fichier spécifique, pour afficher
        /// des résultats de recherche, etc.
        /// </summary>
        /// <param name="e">Détails concernant la requête et le processus de lancement.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = CreateRootFrame();
            RestoreStatus(e.PreviousExecutionState);

            //MainPage is always in rootFrame so we don't have to worry about restoring the navigation state on resume
            rootFrame.Navigate(typeof(MainPage), e.Arguments);

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Restaure les transitions de contenu une fois l'application lancée.
        /// </summary>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        public static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        /// <summary>
        /// Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
        /// sans savoir si l'application pourra se fermer ou reprendre sans endommager
        /// le contenu de la mémoire.
        /// </summary>
        /// <param name="sender">Source de la requête de suspension.</param>
        /// <param name="e">Détails de la requête de suspension.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            localSettings.Values["PreviousMapCenterLat"] = MainPage.Map.Center.Position.Latitude;
            localSettings.Values["PreviousMapCenterLon"] = MainPage.Map.Center.Position.Longitude;
            localSettings.Values["PreviousMapZoom"] = MainPage.Map.ZoomLevel;
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
        void App_Resuming(object sender, object e)
        {
            //if (DateTime.Now.Hour > 20 || DateTime.Now.Hour < 5)
            //    MainPage.Map.ColorScheme = MapColorScheme.Dark;
        }


        protected override void OnActivated(IActivatedEventArgs args)
        {
                base.OnActivated(args);


                if (args.Kind == ActivationKind.Protocol)
                {
                    ProtocolActivatedEventArgs protocolArgs = args as ProtocolActivatedEventArgs;
                    Frame rootFrame = CreateRootFrame();
                    RestoreStatus(args.PreviousExecutionState);

                    if (rootFrame.Content == null)
                    {
                        if (!rootFrame.Navigate(typeof(MainPage)))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }
                    double lat=0, lon = 0;
                    string pattern = @"(?<=lat=)-?[0-9]\d*\.\d+";
                    if (Regex.IsMatch(protocolArgs.Uri.Query, pattern))
                    {
                        var regex = new Regex(pattern).Match(protocolArgs.Uri.Query);
                        if (regex != null && regex.Captures.Count > 0)
                        {
                            lat = double.Parse(regex.Captures[0].Value);
                        }
                    }
                    pattern = @"(?<=lon=)-?[0-9]\d*\.\d+";
                    if (Regex.IsMatch(protocolArgs.Uri.Query, pattern))
                    {
                        var regex = new Regex(pattern).Match(protocolArgs.Uri.Query);
                        if (regex != null && regex.Captures.Count > 0)
                        {
                            lon= double.Parse(regex.Captures[0].Value);
                        }
                    }

                    
                    // Ensure the current window is active
                    Window.Current.Activate();

                    var p = rootFrame.Content as MainPage;
                    if (p != null) {
                        if (MainPage.Map != null)
                        {
                            if (DateTime.Now.Hour > 20 || DateTime.Now.Hour < 5)
                                MainPage.Map.ColorScheme = MapColorScheme.Dark;
                        }

                    if (lat != 0 && lon != 0)
                    {
                        p.SetViewToLocation(lat, lon);
                    }
                    else
                    {
                        var dialog = new MessageDialog("Unable to find the passed location :(");
                        dialog.ShowAsync();
                    }
                    }
                }

        }
    }
}
