using System.Collections.Generic;

using ESRI.ArcGIS.Client.Geometry;

namespace ArcFMSilverlight
{
    /// <summary>
    /// Manages map extents.
    /// </summary>
    public class Extents
    {
        #region private declarations

        List<Envelope> _extentHistory;
        private int _currentExtentIndex;

        #endregion private declarations

        #region ctor
        public Extents()
        {
            _extentHistory = new List<Envelope>();
            _currentExtentIndex = -1;
        }

        public Extents(Envelope extent)
        {
            _extentHistory = new List<Envelope>();
            _currentExtentIndex = -1;

            if (extent != null)
            {
                _extentHistory.Add(extent);
                _currentExtentIndex++;
            }
        }

        #endregion ctor

        #region public properties/methods

        public bool HasPreviousExtent
        {
            get
            {
                if (_extentHistory.Count <= 0)
                {
                    return false;
                }
                else if (_currentExtentIndex > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool HasNextExtent
        {
            get
            {
                int currentExtentIndex = _currentExtentIndex;
                if (_extentHistory.Count <= 0)
                {
                    return false;
                }
                        //0 based               1 based
                else if (_currentExtentIndex >= (_extentHistory.Count - 1))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public Envelope PreviousExtent
        {
            get
            {
                if (HasPreviousExtent)
                {
                    _currentExtentIndex--;
                    return _extentHistory[_currentExtentIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        public Envelope NextExtent
        {
            get
            {
                if (HasNextExtent)
                {
                    _currentExtentIndex++;
                    return _extentHistory[_currentExtentIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        public void Add(Envelope extent)
        {
            if (extent == null) return;

            ++_currentExtentIndex;
            //Remove all of the extents at the index and up
            int historyCount = _extentHistory.Count;
            _extentHistory.RemoveRange(_currentExtentIndex, historyCount - _currentExtentIndex);
            _extentHistory.Add(extent);
        }

        #endregion public properties/methods
    }
}
