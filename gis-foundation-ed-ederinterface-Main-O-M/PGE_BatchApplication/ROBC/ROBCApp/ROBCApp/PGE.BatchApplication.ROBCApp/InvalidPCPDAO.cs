using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ROBCApp
{
    public static class InvalidPCPDAO
    {
        private static ObservableCollection<InvalidPCP> InvalidPCPList = new ObservableCollection<InvalidPCP>
        {
          new InvalidPCP(1,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","A"),
          new InvalidPCP(2,42271102,"Recloser","462","APPLE HILL 1103",string.Empty,string.Empty,"E","B"),
          new InvalidPCP(3,152811107,"Switch","462","ARBUCKLE 1101",string.Empty,string.Empty,"E","Z"),
          new InvalidPCP(4,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","D"),
          new InvalidPCP(5,42271102,"Recloser","462","APPLE HILL 1103",string.Empty,string.Empty,"E","E"),
          new InvalidPCP(6,152811107,"Switch","462","ARBUCKLE 1101",string.Empty,string.Empty,"E","F"),
          new InvalidPCP(7,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","G"),
          new InvalidPCP(8,42271102,"Recloser","462","APPLE HILL 1103",string.Empty,string.Empty,"E","H"),
          new InvalidPCP(9,152811107,"Switch","462","ARBUCKLE 1101",string.Empty,string.Empty,"E","I"),
          new InvalidPCP(10,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","J"),
          new InvalidPCP(11,42271102,"Recloser","462","APPLE HILL 1103",string.Empty,string.Empty,"E","K"),
          new InvalidPCP(12,152811107,"Switch","462","ARBUCKLE 1101",string.Empty,string.Empty,"E","L"),
          new InvalidPCP(13,42811112,"Recloser","120","ALLEGHANY 1101",string.Empty,string.Empty,"E","M"),
          new InvalidPCP(14,42271102,"Recloser","462","APPLE HILL 1103",string.Empty,string.Empty,"E","N"),
          new InvalidPCP(15,152811107,"Switch","462","ARBUCKLE 1101",string.Empty,string.Empty,"E","O")
        };

        public static ObservableCollection<InvalidPCP> GetProducts(int start, int itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            totalItems = InvalidPCPList.Count;
            ObservableCollection<InvalidPCP> sortedProducts = new ObservableCollection<InvalidPCP>();
            switch (sortColumn)
            {
                case ("Id"):
                    sortedProducts = new ObservableCollection<InvalidPCP>
                    (
                      from p in InvalidPCPList
                      orderby p.Id
                      select p
                    );
                    break;
                case ("CircuitId"):
                    sortedProducts = new ObservableCollection<InvalidPCP>
                    (
                      from p in InvalidPCPList
                      orderby p.CircuitId
                      select p
                    );
                    break;

                case ("FeederName"):
                    sortedProducts = new ObservableCollection<InvalidPCP>
                    (
                      from p in InvalidPCPList
                      orderby p.FeederName
                      select p
                    );
                    break;
                case ("ROBC"):
                    sortedProducts = new ObservableCollection<InvalidPCP>
                    (
                      from p in InvalidPCPList
                      orderby p.DesiredROBC
                      select p
                    );
                    break;
                case ("SubBlock"):
                    sortedProducts = new ObservableCollection<InvalidPCP>
                    (
                      from p in InvalidPCPList
                      orderby p.DesiredSubBlock
                      select p
                    );
                    break;
                case ("EstablishedROBC"):
                    sortedProducts = new ObservableCollection<InvalidPCP>
                    (
                      from p in InvalidPCPList
                      orderby p.DesiredROBC
                      select p
                    );
                    break;
                case ("EstablishedSubBlock"):
                    sortedProducts = new ObservableCollection<InvalidPCP>
                    (
                      from p in InvalidPCPList
                      orderby p.DesiredSubBlock
                      select p
                    );
                    break;

                case ("DeviceType"):
                    sortedProducts = new ObservableCollection<InvalidPCP>
                    (
                      from p in InvalidPCPList
                      orderby p.DeviceType
                      select p
                    );
                    break;
            }

            sortedProducts = ascending ? sortedProducts : new ObservableCollection<InvalidPCP>(sortedProducts.Reverse());
            ObservableCollection<InvalidPCP> filteredProducts = new ObservableCollection<InvalidPCP>();
            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredProducts.Add(sortedProducts[i]);
            }
            return filteredProducts;
        }

    }
}
