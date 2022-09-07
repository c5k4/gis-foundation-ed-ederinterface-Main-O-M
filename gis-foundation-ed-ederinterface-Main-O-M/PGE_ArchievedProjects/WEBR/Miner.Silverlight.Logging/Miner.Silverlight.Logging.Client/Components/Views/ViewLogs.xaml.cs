using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using NLog;

namespace Miner.Silverlight.Logging.Client.Components.Views
{
    public partial class ViewLogs : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
    
        public ViewLogs()
        {
            InitializeComponent();
            this.Loaded += OnLogViewerLoaded;
        }

        
        private void OnLogViewerLoaded(object sender, RoutedEventArgs e)
        {
            logger.Info("Log Viewer Started");
            ((FloatableWindow)this.Parent).SizeChanged += OnParentResize;
            OnParentResize(null, null);

            //Get local log file
            String localFileName = LogHelper.DefaultLocalLogFileName;
            StringBuilder sb = new StringBuilder();

            using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                for (var idx = 2; idx > -1; idx--)
                {
                    ReadLogFile(isolatedStorage, localFileName, sb, idx);
                }
            }

            this.FrmMsgBox.Text = sb.ToString();
            this.FrmMsgBox.Focus();
            this.FrmMsgBox.SelectionStart = sb.Length;

            logger.Info("Log view successfully loaded");
        }

        public void OnParentResize(object sender, SizeChangedEventArgs e)
        {
            // Size txt box
            FrmMsgBox.Height = ((FloatableWindow) this.Parent).ActualHeight;
            FrmMsgBox.Width = ((FloatableWindow) this.Parent).ActualWidth-16;
        }

        private static void ReadLogFile(IsolatedStorageFile isolatedStorage, string localFileName, StringBuilder sb, int idx)
        {
            string curFileName = localFileName + ((idx != 0) ? idx.ToString() : "") + ".log";
            if (isolatedStorage.FileExists(curFileName))
            {
                using (
                    IsolatedStorageFileStream stream = new IsolatedStorageFileStream(curFileName,
                                                                                     FileMode.Open,
                                                                                     FileAccess.Read,
                                                                                     isolatedStorage))
                {
                    TextReader tr = new StreamReader(stream);
                    sb.Append(tr.ReadToEnd());
                }
            }
        }
    }
}
