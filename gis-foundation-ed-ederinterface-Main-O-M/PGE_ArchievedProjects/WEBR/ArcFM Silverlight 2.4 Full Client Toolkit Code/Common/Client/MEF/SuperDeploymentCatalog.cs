//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
//
// Modified: 10/22/2010
// By: Chris Pietschmann (http://pietschsoft.com)
// The DeploymentCatalog class was modified to allow for not only
// downloading XAP files, but also individual DLL's. The changes to
// the DeploymentCatalog class to allow this were implemented within
// the "HandleOpenReadCompleted" method.
//
// This modification was originally posted on Chris Pietschmann's
// blog at http://pietschsoft.com
//

using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Resources;
using System.Xml;

namespace System.ComponentModel.Composition.Hosting
{
    internal class SuperDeploymentCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        // Fields
        private AggregateCatalog _catalogCollection;
        private volatile bool _isDisposed;
        private Lock _lock;
        private int _state;
        private Uri _uri;
        private WebClient _webClient;

        // Events
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        public event EventHandler<AsyncCompletedEventArgs> DownloadCompleted;

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        // Methods
        public SuperDeploymentCatalog()
        {
            this._lock = new Lock();
            this._isDisposed = false;
            this._uri = null;
            this._state = 0;
            this._catalogCollection = new AggregateCatalog();
            this._webClient = null;
            this.DiscoverParts(Package.CurrentAssemblies);
            this._state = 0x3e8;
        }

        public SuperDeploymentCatalog(string uriRelative)
        {
            this._lock = new Lock();
            this._isDisposed = false;
            this._uri = null;
            this._state = 0;
            this._catalogCollection = new AggregateCatalog();
            this._webClient = null;
            Requires.NotNullOrEmpty(uriRelative, "uriRelative");
            this._uri = new Uri(uriRelative, UriKind.Relative);
        }

        public SuperDeploymentCatalog(Uri uri)
        {
            this._lock = new Lock();
            this._isDisposed = false;
            this._uri = null;
            this._state = 0;
            this._catalogCollection = new AggregateCatalog();
            this._webClient = null;
            Requires.NotNull<Uri>(uri, "uri");
            this._uri = uri;
        }

        public void CancelAsync()
        {
            this.ThrowIfDisposed();
            this.MutateStateOrThrow(0xfa0, 0x7d0);
            this.WebClient.CancelAsync();
        }

        private void DiscoverParts(IEnumerable<Assembly> assemblies)
        {
            this.ThrowIfDisposed();
            List<ComposablePartDefinition> addedDefinitions = new List<ComposablePartDefinition>();
            Dictionary<string, ComposablePartCatalog> dictionary = new Dictionary<string, ComposablePartCatalog>();
            using (new ReadLock(this._lock))
            {
                foreach (Assembly assembly in assemblies)
                {
                    if (!dictionary.ContainsKey(assembly.FullName))
                    {
                        AssemblyCatalog catalog = new AssemblyCatalog(assembly);
                        addedDefinitions.AddRange(catalog.Parts);
                        dictionary.Add(assembly.FullName, catalog);
                    }
                }
            }
            using (AtomicComposition composition = new AtomicComposition())
            {
                ComposablePartCatalogChangeEventArgs args = new ComposablePartCatalogChangeEventArgs(addedDefinitions, Enumerable.Empty<ComposablePartDefinition>(), composition);
                this.OnChanging(args);
                using (new WriteLock(this._lock))
                {
                    foreach (KeyValuePair<string, ComposablePartCatalog> pair in dictionary)
                    {
                        this._catalogCollection.Catalogs.Add(pair.Value);
                    }
                }
                composition.Complete();
            }
            ComposablePartCatalogChangeEventArgs e = new ComposablePartCatalogChangeEventArgs(addedDefinitions, Enumerable.Empty<ComposablePartDefinition>(), null);
            this.OnChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !this._isDisposed)
                {
                    AggregateCatalog catalog = null;
                    bool flag = false;
                    try
                    {
                        using (new WriteLock(this._lock))
                        {
                            if (!this._isDisposed)
                            {
                                flag = true;
                                catalog = this._catalogCollection;
                                this._catalogCollection = null;
                                this._isDisposed = true;
                            }
                        }
                    }
                    finally
                    {
                        if (catalog != null)
                        {
                            catalog.Dispose();
                        }
                        if (flag)
                        {
                            this._lock.Dispose();
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public void DownloadAsync()
        {
            this.ThrowIfDisposed();
            if (Interlocked.CompareExchange(ref this._state, 0x7d0, 0) == 0)
            {
                this.WebClient.OpenReadCompleted += new OpenReadCompletedEventHandler(this.HandleOpenReadCompleted);
                this.WebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.HandleDownloadProgressChanged);
                this.WebClient.OpenReadAsync(this.Uri, this);
            }
            else
            {
                this.MutateStateOrThrow(0xbb8, 0x3e8);
                this.OnDownloadCompleted(new AsyncCompletedEventArgs(null, false, this));
            }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            this.ThrowIfDisposed();
            Requires.NotNull<ImportDefinition>(definition, "definition");
            return this._catalogCollection.GetExports(definition);
        }

        private void HandleDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            EventHandler<DownloadProgressChangedEventArgs> downloadProgressChanged = this.DownloadProgressChanged;
            if (downloadProgressChanged != null)
            {
                downloadProgressChanged(this, e);
            }
        }

        private void HandleOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            Exception error = e.Error;
            bool cancelled = e.Cancelled;
            if (Interlocked.CompareExchange(ref this._state, 0xbb8, 0x7d0) != 0x7d0)
            {
                cancelled = true;
            }
            if ((error == null) && !cancelled)
            {
                try
                {
                    // Check if a XAP or DLL was downloaded, based on file extension
                    if (this._uri.OriginalString.ToLower().EndsWith(".xap"))
                    {
                        // Load Assemblies from XAP
                        IEnumerable<Assembly> assemblies = Package.LoadPackagedAssemblies(e.Result);
                        this.DiscoverParts(assemblies);
                    }
                    else // if (this._uri.OriginalString.ToLower().EndsWith(".dll"))
                    {
                        // Load DLL Assembly
                        var assemblies = new List<Assembly>();
#if SILVERLIGHT
                        assemblies.Add((new AssemblyPart()).Load(e.Result));
#elif WPF
                        var reader = new BinaryReader(e.Result);
                        assemblies.Add(Assembly.Load(reader.ReadBytes((int)reader.BaseStream.Length)));
#endif
                        this.DiscoverParts(assemblies);
                    }
                }
                catch (Exception exception2)
                {
                    error = new InvalidOperationException("Strings.InvalidOperationException_ErrorReadingXap", exception2);
                }
            }
            this.OnDownloadCompleted(new AsyncCompletedEventArgs(error, cancelled, this));
        }

        private void MutateStateOrThrow(int toState, int fromState)
        {
            if (Interlocked.CompareExchange(ref this._state, toState, fromState) != fromState)
            {
                throw new InvalidOperationException("Strings.InvalidOperationException_DeploymentCatalogInvalidStateChange");
            }
        }

        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
        {
            EventHandler<ComposablePartCatalogChangeEventArgs> changed = this.Changed;
            if (changed != null)
            {
                changed(this, e);
            }
        }

        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {
            EventHandler<ComposablePartCatalogChangeEventArgs> changing = this.Changing;
            if (changing != null)
            {
                changing(this, e);
            }
        }

        protected virtual void OnDownloadCompleted(AsyncCompletedEventArgs e)
        {
            EventHandler<AsyncCompletedEventArgs> downloadCompleted = this.DownloadCompleted;
            if (downloadCompleted != null)
            {
                downloadCompleted(this, e);
            }
        }

        protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            EventHandler<DownloadProgressChangedEventArgs> downloadProgressChanged = this.DownloadProgressChanged;
            if (downloadProgressChanged != null)
            {
                downloadProgressChanged(this, e);
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(base.GetType().ToString());
            }
        }

        // Properties
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                this.ThrowIfDisposed();
                return this._catalogCollection.Parts;
            }
        }

        public Uri Uri
        {
            get
            {
                this.ThrowIfDisposed();
                return this._uri;
            }
        }

        private WebClient WebClient
        {
            get
            {
                this.ThrowIfDisposed();
                if (this._webClient == null)
                {
                    Interlocked.CompareExchange<WebClient>(ref this._webClient, new WebClient(), null);
                }
                return this._webClient;
            }
        }

        // Nested Types
        private static class State
        {
            // Fields
            public const int Created = 0;
            public const int DownloadCancelled = 0xfa0;
            public const int DownloadCompleted = 0xbb8;
            public const int DownloadStarted = 0x7d0;
            public const int Initialized = 0x3e8;
        }

        #region "Internal"

        internal static class Package
        {
            // Methods
#if SILVERLIGHT
            private static IEnumerable<AssemblyPart> GetDeploymentParts(StreamResourceInfo xapStreamInfo)
#elif WPF
            private static IEnumerable<Assembly> GetDeploymentParts(StreamResourceInfo xapStreamInfo)
#endif
            {
                Uri uriResource = new Uri("AppManifest.xaml", UriKind.Relative);
#if SILVERLIGHT
                StreamResourceInfo resourceStream = Application.GetResourceStream(xapStreamInfo, uriResource);
                List<AssemblyPart> list = new List<AssemblyPart>();
#elif WPF
                StreamResourceInfo resourceStream = Application.GetResourceStream(uriResource);
                List<Assembly> list = new List<Assembly>();
#endif
                if (resourceStream != null)
                {
                    using (XmlReader reader = XmlReader.Create(resourceStream.Stream))
                    {
                        if (!reader.ReadToFollowing("AssemblyPart"))
                        {
                            return list;
                        }

                        do
                        {
                            string attribute = reader.GetAttribute("Source");
                            if (attribute != null)
                            {
#if SILVERLIGHT
                                list.Add(new AssemblyPart { Source = attribute });
#elif WPF
                                list.Add(Assembly.Load(attribute));
#endif
                            }
                        }
                        while (reader.ReadToNextSibling("AssemblyPart"));
                    }
                }
                return list;
            }

            public static IEnumerable<Assembly> LoadPackagedAssemblies(Stream packageStream)
            {
                List<Assembly> list = new List<Assembly>();
                StreamResourceInfo xapStreamInfo = new StreamResourceInfo(packageStream, null);
#if SILVERLIGHT
                IEnumerable<AssemblyPart> deploymentParts = GetDeploymentParts(xapStreamInfo);
                foreach (AssemblyPart part in deploymentParts)
                {
                    StreamResourceInfo resourceStream = Application.GetResourceStream(xapStreamInfo, new Uri(part.Source, UriKind.Relative));
                    list.Add(part.Load(resourceStream.Stream));
                }
#elif WPF
                IEnumerable<Assembly> deploymentParts = GetDeploymentParts(xapStreamInfo);
                foreach (Assembly part in deploymentParts)
                {
                    var resourceStream = Application.GetResourceStream(new Uri(part.FullName, UriKind.Relative));
                    var reader = new BinaryReader(resourceStream.Stream);
                    list.Add(Assembly.Load(reader.ReadBytes((int)reader.BaseStream.Length)));
                }
#endif
                packageStream.Close();

                return list;
            }

            // Properties
            public static IEnumerable<Assembly> CurrentAssemblies
            {
                get
                {
#if SILVERLIGHT
                    List<Assembly> list = new List<Assembly>();
                    foreach (AssemblyPart part in Deployment.Current.Parts)
                    {
                        StreamResourceInfo resourceStream = Application.GetResourceStream(new Uri(part.Source, UriKind.Relative));
                        if (resourceStream != null)
                        {
                            Assembly item = part.Load(resourceStream.Stream);
                            list.Add(item);
                        }
                    }

                    return list;
#elif WPF
                    return AppDomain.CurrentDomain.GetAssemblies().ToList();
#endif
                }
            }
        }

        internal static class Requires
        {
            // Methods
            [DebuggerStepThrough]
            public static void NotNull<T>(T value, string parameterName) where T : class
            {
                if (value == null)
                {
                    throw new ArgumentNullException(parameterName);
                }
            }

            [DebuggerStepThrough]
            public static void NotNullOrEmpty(string value, string parameterName)
            {
                NotNull<string>(value, parameterName);
                if (value.Length == 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Strings.ArgumentException_EmptyString", new object[] { parameterName }), parameterName);
                }
            }
        }

        internal sealed class Lock : IDisposable
        {
            // Fields
            private object _thisLock = new object();

            // Methods
            public void Dispose()
            {
            }

            public void EnterReadLock()
            {
                Monitor.Enter(this._thisLock);
            }

            public void EnterWriteLock()
            {
                Monitor.Enter(this._thisLock);
            }

            public void ExitReadLock()
            {
                Monitor.Exit(this._thisLock);
            }

            public void ExitWriteLock()
            {
                Monitor.Exit(this._thisLock);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ReadLock : IDisposable
        {
            private readonly Lock _lock;
            private int _isDisposed;
            public ReadLock(Lock @lock)
            {
                this._isDisposed = 0;
                this._lock = @lock;
                this._lock.EnterReadLock();
            }

            public void Dispose()
            {
                if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
                {
                    this._lock.ExitReadLock();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WriteLock : IDisposable
        {
            private readonly Lock _lock;
            private int _isDisposed;
            public WriteLock(Lock @lock)
            {
                this._isDisposed = 0;
                this._lock = @lock;
                this._lock.EnterWriteLock();
            }

            public void Dispose()
            {
                if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
                {
                    this._lock.ExitWriteLock();
                }
            }
        }
        #endregion
    }

}


