using System.IO;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <exclude/>
    public class DefaultExportToExcelItem : ICustomizeExportToExcel
    {
        public string Name
        {
            get { return "Default Format"; }
        }
       
        public Stream Convert(Stream inStream)
        {
            return inStream;
        }
    }
}

