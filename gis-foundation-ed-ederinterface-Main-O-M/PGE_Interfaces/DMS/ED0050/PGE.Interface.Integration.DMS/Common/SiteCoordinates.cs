using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;

namespace PGE.Interface.Integration.DMS.Common
{
    public class SiteCoordinates
    {
        private double _minX, _maxX, _minY, _maxY;
        private GeoPoint _center;
        private bool _static;
        private CoincidentNode _coinNode;
        private int _dmsX, _dmsY;

        public CoincidentNode CoinNode
        {
            get { return _coinNode; }
        }

        public SiteCoordinates(ObjectInfo devgroup)
        {
            //start at the device groups XY
            _center = (GeoPoint)((SimpleFeatureInfo)devgroup).Shape;
            _minX = _center.X;
            _minY = _center.Y;
            _maxX = _minX;
            _maxY = _minY;
            _coinNode = new CoincidentNode(_minX, _minY, 0);
            _dmsX = Utilities.ConvertXY(_center.X);
            _dmsY = Utilities.ConvertXY(_center.Y);
        }
        public bool IsCoincident(GeoPoint point)
        {
            return _dmsX == Utilities.ConvertXY(point.X) && _dmsY == Utilities.ConvertXY(point.Y);
        }
        public void AddStaticSize(SiteSize size)
        {
            double xdim = size.Width / 2;
            _minX = _center.X - xdim;
            _maxX = _center.X + xdim;
            double ydim = size.Height / 2;
            _minY = _center.Y - ydim;
            _maxY = _center.Y + ydim;
            _static = true;
        }
        public void addJunction(JunctionFeatureInfo junct)
        {
            if (!_static)
            {
                double x = junct.Junction.Point.X;
                double y = junct.Junction.Point.Y;
                addXY(x, y);
            }
        }
        public void AddEdge(EdgeFeatureInfo edge)
        {
            if (!_static)
            {
                foreach (EdgeInfo e in edge.Edges)
                {
                    foreach (GeoPoint p in e.Line)
                    {
                        addXY(p.X, p.Y);
                    }
                }
            }
        }
        private void addXY(double x, double y)
        {
            if (x < _minX)
            {
                _minX = x;
            }
            else if (x > _maxX)
            {
                _maxX = x;
            }
            if (y < _minY)
            {
                _minY = y;
            }
            else if (y > _maxY)
            {
                _maxY = y;
            }
        }
        public int X
        {
            get
            {
                return Utilities.ConvertXY(_minX) - Configuration.Buffer;
            }
        }
        public int Y
        {
            get
            {
                return Utilities.ConvertXY(_maxY) + Configuration.Buffer;
            }
        }
        public int Width
        {
            get
            {
                return (Utilities.ConvertXY(_maxX) - Utilities.ConvertXY(_minX) + Configuration.Buffer*2);
            }

        }
        public int Height
        {
            get
            {
                return (Utilities.ConvertXY(_maxY) - Utilities.ConvertXY(_minY) + Configuration.Buffer*2);
            }
        }
    }
}
