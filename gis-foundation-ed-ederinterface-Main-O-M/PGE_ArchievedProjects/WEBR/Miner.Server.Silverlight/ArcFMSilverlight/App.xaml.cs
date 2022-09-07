using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ArcFM.Silverlight.PGE.CustomTools;
using Miner.Silverlight.Logging.Client;
using NLog;
using System.ServiceModel.DomainServices.Client.ApplicationServices;

namespace ArcFMSilverlight
{
    public partial class App : Application
    {
        private Logger logger;
        
        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();

            // Create a WebContext and add it to the ApplicationLifetimeObjects
            // collection.  This will then be available as WebContext.Current.
            var webContext = new WebContext { Authentication = new WindowsAuthentication() };
            ApplicationLifetimeObjects.Add(webContext);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LogHelper.InitializeNLog();
            this.logger = LogManager.GetCurrentClassLogger();
            logger.Info("Test System Log");

            // This will enable you to bind controls in XAML files to WebContext.Current
            // properties
            Resources.Add("WebContext", WebContext.Current);

            // This will automatically authenticate a user when using windows authentication
            // or when the user chose "Keep me signed in" on a previous login attempt
            WebContext.Current.Authentication.LoadUser(this.Application_UserLoaded, null);

            InitializeRootVisual();
            //this.RootVisual = new MainPage();
        }

        /// <summary>
        /// Invoked when the <see cref="LoadUserOperation"/> completes. Use this
        /// event handler to switch from the "loading UI" you created in
        /// <see cref="InitializeRootVisual"/> to the "application UI"
        /// </summary>
        private void Application_UserLoaded(LoadUserOperation operation)
        {
            RootVisual = new MainPage();
        }

        /// <summary>
        /// Initializes the <see cref="Application.RootVisual"/> property. The
        /// initial UI will be displayed before the LoadUser operation has completed
        /// (The LoadUser operation will cause user to be logged automatically if
        /// using windows authentication or if the user had selected the "keep
        /// me signed in" option on a previous login).
        /// </summary>
        protected virtual void InitializeRootVisual()
        {

        }
        private void Application_Exit(object sender, EventArgs e)
        {
            GeographicSearchFilterControl.SaveGeographicFilterLocation();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            //ignore known product issues, there is a ticket in for this so hopefully they will fix it some day
            //even though the product component is throwing an exception, it does not seem to cause any problems in the client
            if (e.ExceptionObject != null)
            {
                if (e.ExceptionObject.Message != null)
                {
                    if (e.ExceptionObject.Message.Equals("Object reference not set to an instance of an object."))
                    {
                        if (e.ExceptionObject.StackTrace != null)
                        {
                            if (e.ExceptionObject.StackTrace.Contains("Miner.Server.Client.ConfiguredRelationshipTreeItem.CallRelationshipService") ||
                                e.ExceptionObject.StackTrace.Contains("Miner.Server.Client.RelationshipTreeItem.RelationshipInfoServiceCompleted"))
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                    }
                }
            }

            logger.FatalException("Fatal Error: (Client Version 1.9.5)" + e.ExceptionObject.Message + " -- " + e.ExceptionObject.StackTrace, e.ExceptionObject);

            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                //this already gets logged in Application_UnhandledException JR
                //logger.FatalException("Fatal Error: " + e.ExceptionObject.Message + " -- " + e.ExceptionObject.StackTrace, e.ExceptionObject);

                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
