using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;

namespace PGE.Interface.Integration.DMS.Common
{
    /// <summary>
    /// Data class used to store x,y and ID information for a node
    /// </summary>
    public class CoincidentNode : IComparable
    {
        private int _x;
        private int _y;
        private double _iD;
        private string _hash;

        /// <summary>
        /// Initialize the CoincidentNode with the values of a junction
        /// </summary>
        /// <param name="junct">The junction to pull the values off of</param>
        public CoincidentNode(JunctionFeatureInfo junct)
        {
            _x = Utilities.ConvertXY(junct.Junction.Point.X);
            _y = Utilities.ConvertXY(junct.Junction.Point.Y);
            _iD = Utilities.getID(junct);
            _hash = _x + "-" + _y;
        }
        /// <summary>
        /// Initialize the CoincidentNode with static values
        /// </summary>
        /// <param name="x">The node's x coordinate</param>
        /// <param name="y">The node's y coordinate</param>
        /// <param name="id">The unique staging schema node id</param>
        public CoincidentNode(double x, double y, int id)
        {
            _x = Utilities.ConvertXY(x);
            _y = Utilities.ConvertXY(y);
            _iD = id;
            _hash = _x + "-" + _y;
        }
        /// <summary>
        /// Compare x and y values to see if the two node are equal i.e. coincident
        /// </summary>
        /// <param name="obj">The node to compare this one to</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is CoincidentNode)
            {
                CoincidentNode c = (CoincidentNode)obj;
                return c.X == _x && c.Y == _y;
            }
            return false;
        }
        /// <summary>
        /// Generate a special hashcode so this can be more efficiently managed in dictionaries
        /// </summary>
        /// <returns>The hashcode based on x/y values</returns>
        public override int GetHashCode()
        {
            return _hash.GetHashCode();
        }
        /// <summary>
        /// The unique ID of the node
        /// </summary>
        public double ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        /// <summary>
        /// The Y coordinate of the node
        /// </summary>
        public int Y
        {
            get { return _y; }
            set
            {
                _y = value;
                _hash = _x + "-" + _y;
            }
        }
        /// <summary>
        /// The X coordinate of the node
        /// </summary>
        public int X
        {
            get { return _x; }
            set
            {
                _x = value;
                _hash = _x + "-" + _y;
            }
        }

        #region IComparable Members
        /// <summary>
        /// Can be used to sort CoincidentNode objects based on their X values then Y values usefully if you want to perform a binary search on the X coordinate
        /// </summary>
        /// <param name="obj">CoincidentNode to compare to</param>
        /// <returns>-1 if less, 0 if equal or can't compare, 1 if greater</returns>
        public int CompareTo(object obj)
        {
            if (obj is CoincidentNode)
            {
                CoincidentNode c = (CoincidentNode)obj;
                if (_x < c.X)
                {
                    return -1;
                }
                else if (_x > c.X)
                {
                    return 1;
                }
                else
                {
                    if (_y < c.Y)
                    {
                        return -1;
                    }
                    else if (_y > c.Y)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        #endregion
    }
}
