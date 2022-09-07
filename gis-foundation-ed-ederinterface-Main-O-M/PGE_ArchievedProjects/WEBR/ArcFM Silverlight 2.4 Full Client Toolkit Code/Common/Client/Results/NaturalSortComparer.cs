﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    // implementation of NaturalSort from:
    //      http://www.codeproject.com/KB/string/NaturalSortComparer.aspx

    internal class NaturalSortComparer<T> : IComparer<object>, IDisposable
    {
        #region IComparer<string> Members
        public int Compare(string x, string y)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IComparer<string> Members
        int IComparer<object>.Compare(object xObj, object yObj)
        {
            string x = Convert.ToString(xObj);
            string y = Convert.ToString(yObj);

            if (x == y)
            {
                return 0;
            }

            // Short circuit the natural sort if the types are dates
            DateTime outDateX;
            DateTime outDateY;
            if ((DateTime.TryParse(x, out outDateX) && DateTime.TryParse(y, out outDateY)))
            {
                return outDateX.CompareTo(outDateY);
            }

            string[] x1, y1;
            if (!table.TryGetValue(x, out x1))
            {
                x1 = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
                table.Add(x, x1);
            }
            if (!table.TryGetValue(y, out y1)) 
            { 
                y1 = Regex.Split(y.Replace(" ", ""), "([0-9]+)"); 
                table.Add(y, y1);
            }
            for (int i = 0; i < x1.Length && i < y1.Length; i++) 
            { 
                if (x1[i] != y1[i])
                {
                    return PartCompare(x1[i], y1[i]);
                }
            }

            if (y1.Length > x1.Length)
            {
                return 1;
            }
            else if (x1.Length > y1.Length) 
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        private static int PartCompare(string left, string right)
        {
            int x, y;
            if (!int.TryParse(left, out x))
            {
                return left.CompareTo(right);
            }

            if (!int.TryParse(right, out y))
            {
                return left.CompareTo(right);
            }

            return x.CompareTo(y);
        }
        #endregion

        private Dictionary<string, string[]> table = new Dictionary<string, string[]>();
        public void Dispose()
        {
            table.Clear();
            table = null;
        }
    }
  
}