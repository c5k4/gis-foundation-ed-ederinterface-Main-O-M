using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGE.BatchApplication.PageDotConfigured.ConfigValidation.Validators;

namespace PGE.BatchApplication.PageDotConfigured.Tests
{
    [TestClass]
    public class ClassIdLayerMapValidatorTests
    {
        [TestMethod]
        public void ClassIdLayerMapValidator_shouldGenerate()
        {
            string mapServiceUrl = "http://wsgo496902:6080/arcgis/rest/services/Data/ElectricDistribution/MapServer";
//            mapServiceUrl = "http://wsgo496902:6080/arcgis/rest/services/Data/Publication/MapServer";
            ClassIdLayerMapValidator validator = new ClassIdLayerMapValidator();

            validator.Generate(mapServiceUrl);

            Assert.IsTrue(validator.Mappings[mapServiceUrl].Count > 0);
            Assert.IsTrue(validator.ClassIdToLayerMapElement.HasElements);

        }

    }
}
