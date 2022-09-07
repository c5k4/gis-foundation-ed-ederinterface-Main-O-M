using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ROBCApp
{
    public static class UnassignedROBCDAO
    {
        private static ObservableCollection<UnassignedROBC> UnassignedROBCList = new ObservableCollection<UnassignedROBC>
        {
          new UnassignedROBC(1,182571101, "ALLEGHANY 1101","SCADA","2","E","1","E","North Valley"),
          new UnassignedROBC(2,182571101, "APPLE HILL 1103","SCADA","2","E","1","E","North Coast"),
          new UnassignedROBC(3,182571101, "ARBUCKLE 1101","Manual","2","E","1","E","Diablo"),
          new UnassignedROBC(4,182571101, "ALLEGHANY 1101","SCADA","2","E","1","E","East Bay"),
          new UnassignedROBC(5,182571101, "ALLEGHANY 1101","SCADA","2","E","1","E","North Valley"),
          new UnassignedROBC(6,182571101, "APPLE HILL 1103","SCADA","2","E","1","E","North Coast"),
          new UnassignedROBC(7,182571101, "ARBUCKLE 1101","Manual","2","E","1","E","Diablo"),
          new UnassignedROBC(8,182571101, "ALLEGHANY 1101","SCADA","2","E","1","E","East Bay"),
          new UnassignedROBC(9,182571101, "ALLEGHANY 1101","SCADA","2","E","1","E","East Bay"),
          new UnassignedROBC(10,182571101, "ARBUCKLE 1101","SCADA","2","E","1","E","North Bay")
        };

        public static ObservableCollection<UnassignedROBC> GetProducts(int start, int itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            totalItems = UnassignedROBCList.Count;
            ObservableCollection<UnassignedROBC> sortedProducts = new ObservableCollection<UnassignedROBC>();
            switch (sortColumn)
            {
                case ("Id"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.Id
                      select p
                    );
                    break;
                case ("CircuitId"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.CircuitId
                      select p
                    );
                    break;
             
                case ("FeederName"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.FeederName
                      select p
                    );
                    break;
                case ("Scada"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.SCADA
                      select p
                    );
                    break;
                case ("ROBC"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.DesiredROBC
                      select p
                    );
                    break;
                case ("SubBlock"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.DesiredSubBlock
                      select p
                    );
                    break;
                case ("EstablishedROBC"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.DesiredROBC
                      select p
                    );
                    break;
                case ("EstablishedSubBlock"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.DesiredSubBlock
                      select p
                    );
                    break;
             
                case ("Division"):
                    sortedProducts = new ObservableCollection<UnassignedROBC>
                    (
                      from p in UnassignedROBCList
                      orderby p.Division
                      select p
                    );
                    break;
            }

            sortedProducts = ascending ? sortedProducts : new ObservableCollection<UnassignedROBC>(sortedProducts.Reverse());
            ObservableCollection<UnassignedROBC> filteredProducts = new ObservableCollection<UnassignedROBC>();
            for (int i = start; i < start + itemCount && i < totalItems; i++)
            {
                filteredProducts.Add(sortedProducts[i]);
            }
            return filteredProducts;
        }

    }
}
