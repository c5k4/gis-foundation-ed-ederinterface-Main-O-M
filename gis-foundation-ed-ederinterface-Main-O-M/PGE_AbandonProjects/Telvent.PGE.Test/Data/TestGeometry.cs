#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace Telvent.PGE.Test.Data
{
    public class TestGeometry : IGeometry, IPoint, IEnvelope 
    {
        private double _x, _y, _z, _m;

        public TestGeometry(double x, double y)
        {
            _x = x;
            _y = y;
        }

        #region IGeometry Members

        public esriGeometryDimension Dimension
        {
            get { throw new NotImplementedException(); }
        }

        public IEnvelope Envelope
        {
            get { return this; }
        }

        public void GeoNormalize()
        {
            throw new NotImplementedException();
        }

        public void GeoNormalizeFromLongitude(double Longitude)
        {
            throw new NotImplementedException();
        }

        public esriGeometryType GeometryType
        {
            get { return esriGeometryType.esriGeometryPoint; }
        }

        public bool IsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public void Project(ISpatialReference newReferenceSystem)
        {
            throw new NotImplementedException();
        }

        public void QueryEnvelope(IEnvelope outEnvelope)
        {
            throw new NotImplementedException();
        }

        public void SetEmpty()
        {
            throw new NotImplementedException();
        }

        public void SnapToSpatialReference()
        {
            throw new NotImplementedException();
        }

        public ISpatialReference SpatialReference
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IPoint Members

        public int Compare(IPoint otherPoint)
        {
            throw new NotImplementedException();
        }

        public void ConstrainAngle(double constraintAngle, IPoint anchor, bool allowOpposite)
        {
            throw new NotImplementedException();
        }

        public void ConstrainDistance(double constraintRadius, IPoint anchor)
        {
            throw new NotImplementedException();
        }

        public int ID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double M
        {
            get
            {
                return _m;
            }
            set
            {
                _m = value;
            }
        }

        public void PutCoords(double X, double Y)
        {
            throw new NotImplementedException();
        }

        public void QueryCoords(out double X, out double Y)
        {
            throw new NotImplementedException();
        }

        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }

        public double get_VertexAttribute(esriGeometryAttributes attributeType)
        {
            throw new NotImplementedException();
        }

        public void set_VertexAttribute(esriGeometryAttributes attributeType, double attributeValue)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnvelope Members

        public void CenterAt(IPoint p)
        {
            throw new NotImplementedException();
        }

        public void DefineFromPoints(int Count, ref IPoint Points)
        {
            throw new NotImplementedException();
        }

        public void DefineFromWKSPoints(int Count, ref ESRI.ArcGIS.esriSystem.WKSPoint Points)
        {
            throw new NotImplementedException();
        }

        public double Depth
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Expand(double dx, double dy, bool asRatio)
        {
            throw new NotImplementedException();
        }

        public void ExpandM(double dm, bool asRatio)
        {
            throw new NotImplementedException();
        }

        public void ExpandZ(double dz, bool asRatio)
        {
            throw new NotImplementedException();
        }

        public double Height
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Intersect(IEnvelope inEnvelope)
        {
            throw new NotImplementedException();
        }

        public IPoint LowerLeft
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IPoint LowerRight
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double MMax
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double MMin
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Offset(double X, double Y)
        {
            throw new NotImplementedException();
        }

        public void OffsetM(double M)
        {
            throw new NotImplementedException();
        }

        public void OffsetZ(double Z)
        {
            throw new NotImplementedException();
        }

        public void PutCoords(double XMin, double YMin, double XMax, double YMax)
        {
            throw new NotImplementedException();
        }

        public void PutWKSCoords(ref ESRI.ArcGIS.esriSystem.WKSEnvelope e)
        {
            throw new NotImplementedException();
        }

        public void QueryCoords(out double XMin, out double YMin, out double XMax, out double YMax)
        {
            throw new NotImplementedException();
        }

        public void QueryWKSCoords(out ESRI.ArcGIS.esriSystem.WKSEnvelope e)
        {
            throw new NotImplementedException();
        }

        public void Union(IEnvelope inEnvelope)
        {
            throw new NotImplementedException();
        }

        public IPoint UpperLeft
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IPoint UpperRight
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double Width
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double XMax
        {
            get
            {
                return _x;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double XMin
        {
            get
            {
                return _x;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double YMax
        {
            get
            {
                return _y;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double YMin
        {
            get
            {
                return _y;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double ZMax
        {
            get
            {
                return _z;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double ZMin
        {
            get
            {
                return _z;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IntPtr get_MinMaxAttributes()
        {
            throw new NotImplementedException();
        }

        public void set_MinMaxAttributes(ref ESRI.ArcGIS.esriSystem.esriPointAttributes MinMaxAttributes)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
