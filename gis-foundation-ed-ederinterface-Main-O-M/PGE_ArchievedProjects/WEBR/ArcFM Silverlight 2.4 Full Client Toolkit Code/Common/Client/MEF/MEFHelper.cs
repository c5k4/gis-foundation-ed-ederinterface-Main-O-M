using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
#if WPF
using System.Collections.Generic;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <exclude/>
    public static class MEFHelper
    {
        private static ObservableCollection<IComposable> _composable = new ObservableCollection<IComposable>();
        private static AggregateCatalog _aggregateCatalog;

#if SILVERLIGHT
        public static AggregateCatalog CreateCatalog()
        {
#elif WPF
        private static Dictionary<string, string> _uriRelatives;

        public static AggregateCatalog CreateCatalog(Dictionary<string, string> uriRelatives)
        {
            _uriRelatives = uriRelatives;
#endif
            _aggregateCatalog = new AggregateCatalog();
            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            _aggregateCatalog.Catalogs.Add(assemblyCatalog);

            Composable.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Composable_CollectionChanged);

            return _aggregateCatalog;
        }

        private static void Composable_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.NewItems)
                {
                    IComposable composable = item as IComposable;
                    composable.Catalog = _aggregateCatalog;
#if SILVERLIGHT
                    composable.Compose();
#elif WPF
                    composable.Compose(_uriRelatives);
#endif
                }
            }
        }

        public static ObservableCollection<IComposable> Composable 
        {
            get
            {
                return _composable;
            }
            private set
            {
                _composable = value;
            }
        }

        public static void Initialize()
        {
            CompositionHost.Initialize(_aggregateCatalog);
            foreach (IComposable item in Composable)
            {
                CompositionInitializer.SatisfyImports(item);
            }
        }
    }
}
