using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ROBCApp
{
    public static class MultiplePCPDAO
    {
        private static ObservableCollection<MultiplePCP> MultiplePCPList = new ObservableCollection<MultiplePCP>
        {
          new MultiplePCP(1,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","A"),
          new MultiplePCP(1,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","A"),
          new MultiplePCP(1,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","A"),
          new MultiplePCP(1,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","A")
        
        };

        public static ObservableCollection<MultiplePCP> GetProducts(int start, int itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            totalItems = MultiplePCPList.Count;
            ObservableCollection<MultiplePCP> sortedProducts = new ObservableCollection<MultiplePCP>();
            switch (sortColumn)
            {
                case ("Id"):
                    sortedProducts = new ObservableCollection<MultiplePCP>
                    (
                      from p in MultiplePCPList
                      orderby p.Id
                      select p
                    );
                    break;
                case ("CircuitId"):
                    sortedProducts = new ObservableCollection<MultiplePCP>
                    (
                      from p in MultiplePCPList
                      orderby p.CircuitId
                      select p
                    );
                    break;

                case ("FeederName"):
                    sortedProducts = new ObservableCollection<MultiplePCP>
                    (
                      from p in MultiplePCPList
                      orderby p.FeederName
                      select p
                    );
                    break;
                case ("ROBC"):
                    sortedProducts = new ObservableCollection<MultiplePCP>
                    (
                      from p in MultiplePCPList
                      orderby p.DesiredROBC
                      select p
                    );
                    break;
                case ("SubBlock"):
                    sortedProducts = new ObservableCollection<MultiplePCP>
                    (
                      from p in MultiplePCPList
                      orderby p.DesiredSubBlock
                      select p
                    );
                    break;
                case ("EstablishedROBC"):
                    sortedProducts = new ObservableCollection<MultiplePCP>
                    (
                      from p in MultiplePCPList
                      orderby p.DesiredROBC
                      select p
                    );
                    break;
                case ("EstablishedSubBlock"):
                    sortedProducts = new ObservableCollection<MultiplePCP>
                    (
                      from p in MultiplePCPList
                      orderby p.DesiredSubBlock
                      select p
                    );
                    break;

                case ("DeviceType"):
                    sortedProducts = new ObservableCollection<MultiplePCP>
                    (
                      from p in MultiplePCPList
                      orderby p.DeviceType
                      select p
                    );
                    break;
            }

            sortedProducts = ascending ? sortedProducts : new ObservableCollection<MultiplePCP>(sortedProducts.Reverse());
            ObservableCollection<MultiplePCP> filteredProducts = new ObservableCollection<MultiplePCP>();
            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredProducts.Add(sortedProducts[i]);
            }
            return filteredProducts;
        }

    }
}
