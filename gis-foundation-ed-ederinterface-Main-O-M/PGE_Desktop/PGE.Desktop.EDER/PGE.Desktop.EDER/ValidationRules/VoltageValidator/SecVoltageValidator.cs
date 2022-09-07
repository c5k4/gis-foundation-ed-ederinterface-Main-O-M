using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules.VoltageValidators
{
    /// <summary>
    /// Business logic for performing QAQC on secondary distribution conductors.
    /// </summary>
    public class SecVoltageValidator : VoltageValidator
    {

        /// <summary>
        /// Gets the selected feature which triggered the validator.  Immutable.
        /// </summary>
        public IFeature SourceFeature { get; private set; }

        /// <summary>
        /// Gets the first upstream transformer form the source feature.
        /// </summary>
        public IFeature UpstreamTransformer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecVoltageValidator"/> class.
        /// </summary>
        /// <param name="sourceFeature">The source feature.</param>
        /// <param name="upstreamTransformer">The upstream transformer.</param>
        public SecVoltageValidator(IFeature sourceFeature, IFeature upstreamTransformer)
        {
            this.SourceFeature = sourceFeature;
            this.UpstreamTransformer = upstreamTransformer;
        }

        /// <summary>
        /// Use  SecVoltageValidator(IFeature sourceFeature, IFeature upstreamTransformer)
        /// </summary>
        protected SecVoltageValidator()
        {
        }

        /// <summary>
        /// Validates this instance.  Operating voltage must return the same value as the upstream transformer's low side voltage.
        /// </summary>
        /// <returns></returns>
        public List<string> Validate()
        {

            List<string> errors = new List<string>();

            int xfrOutVoltageIx = ModelNameFacade.FieldIndexFromModelName(UpstreamTransformer.Class, SchemaInfo.Electric.FieldModelNames.SecondaryVoltage);
            int secOHOpVoltageIx = ModelNameFacade.FieldIndexFromModelName(SourceFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);

            int? xfrLowSideVoltage = UpstreamTransformer.get_Value(xfrOutVoltageIx) as int?;
            int? operatingVoltage = SourceFeature.get_Value(secOHOpVoltageIx) as int?;

            if (xfrLowSideVoltage != operatingVoltage)
            {
                errors.Add(buildErrorString("Load line voltage or low side voltage does not match source line and/or secondary line.", UpstreamTransformer));
            }

            return errors;
        }

    }
}
