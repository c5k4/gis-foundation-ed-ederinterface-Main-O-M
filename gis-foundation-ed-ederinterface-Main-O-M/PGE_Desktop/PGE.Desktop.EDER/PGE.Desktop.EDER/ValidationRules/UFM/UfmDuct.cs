using System;
using System.Collections.Generic;
using System.Text;

namespace PGE.Desktop.EDER.ValidationRules.UFM
{
    public class UfmDuct
    {
        #region Member vars

        private string _id;

        // Is the duct occupied
        private bool _ductOccupied;

        // Relative direction from previous duct
        private int _xDir;
        private int _yDir;

        #endregion

        #region Property accessors

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public bool DuctOccupied
        {
            get { return _ductOccupied; }
            set { _ductOccupied = value; }
        }

        public int xDir
        {
            get { return _xDir; }
            set { _xDir = value; }
        }

        public int yDir
        {
            get { return _yDir; }
            set { _yDir = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Two ducts are the same if they have the same relative direction and duct occupancy
        /// </summary>
        /// <param name="compareDuct"></param>
        /// <returns></returns>
        public bool Compare(UfmDuct compareDuct)
        {
            // Assume no match
            bool match = false;

            // If we were supplied a valid duct to compare against...
            if (compareDuct != null)
            {
                // And the the properties match...
                if (_xDir == compareDuct.xDir && _yDir == compareDuct.yDir && _ductOccupied == compareDuct.DuctOccupied)
                {
                    // Then they are the same
                    match = true;
                }
            }

            // Return the result
            return match;
        }

        /// <summary>
        /// Two ducts are mirrored if their relative direction on the x-axis is flipped
        /// </summary>
        /// <param name="compareDuct"></param>
        /// <returns></returns>
        public bool IsMirrored(UfmDuct compareDuct)
        {
            // If we were supplied a valid duct to compare against...
            // The x direction should be mirrored (or match if its neutral)
            // The y direction should be the same...
            // As should be the occupancy
            // Then we match
            if (compareDuct != null &&
                _xDir == compareDuct.xDir &&
                _yDir + compareDuct.yDir == 0 &&
                _ductOccupied == compareDuct.DuctOccupied)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
