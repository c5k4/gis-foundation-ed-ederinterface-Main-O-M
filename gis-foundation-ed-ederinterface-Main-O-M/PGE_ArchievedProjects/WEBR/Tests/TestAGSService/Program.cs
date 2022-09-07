using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CommandFlags;
using System.Threading;
using Telvent.Delivery.Diagnostics;

namespace TestAGSService
{
    class Program
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "TestAGSService.log4net.config");

        private static string _serviceURL;
        private static int _threadCount = 1;
        private static int _jobCount = 1;
        private static string _mode;
        private static int _totalRequests = 0;

        private const string BBOX_FRESNO = "2544064,13328040,2636381,13417232";

        // -s http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer -o exportMap -t 2 -j 1000
        // -s http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer -o getAllDetails -t 2 -j 1000
        // -s http://wsgo496902:6080/arcgis/rest/services/Data/PublicationFS/FeatureServer -o queryGeometry -t 2 -j 1000
        // -s http://wsgo499683:6080/arcgis/rest/services/elec_sub/MapServer -o queryGeometry -t 2 -j 1000
        static void Main(string[] args)
        {
            DateTime exeStart = DateTime.Now;
            try
            {
                //_logger.Debug(args);

                ProcessArguments(args);

                Program program = new Program();
                program.Run();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
            }
            finally
            {
                DateTime exeEnd = DateTime.Now;
                _logger.Info("");
                _logger.Info("Completed");
                _logger.Info("Process start time: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
                _logger.Info("Process end time: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString());
                _logger.Info("Process length: " + (exeEnd - exeStart).ToString());
                _logger.Info("");

#if DEBUG
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
#endif
            }
        }
        public static void ProcessArguments(string[] args)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // -s http://wsgo496902:6080/arcgis/rest/services/Data/ElectricDistribution/MapServer/
            var flags = new FlagParser()
                {
                    //{ "h?", "help", false, false, "Show usage descriptions.", f => _showHelp = f != null },
                    { "s", "serviceURL", true, false, "Service URL", f => _serviceURL = f},
                    { "o", "mode", true, false, "Mode to Execute", f => _mode = f},
                    { "t", "threadCount", true, false, "Thread Count", f => _threadCount = Convert.ToInt32(f)},
                    { "j", "jobCount", true, false, "Job Count", f => _jobCount = Convert.ToInt32(f)}
                };
            flags.Parse(args);

            if (String.IsNullOrEmpty(_serviceURL))  throw new ArgumentException("Invalid Service");

            // Default
            _logger.Debug("Using Service [ " + _serviceURL + " ]");

        }


        public void Run()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
         
//"http://wsgo496902:6080/arcgis/rest/services/Data/ElectricDistribution/MapServer/export?bbox=2812715.1516129%2c12845404%2c2813783.8483871%2c12845948&size=1218%2c620&dpi=96&format=png24&transparent=true&imageSR=%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d&bboxSR=%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d&f=image&"   

            Action action = null;

            switch (_mode.ToUpper())
            {
                case "HITRESTENDPOINT":
                    action = HitRestEndPointAsync;
                    break;
                case "PROJECT":
                    action = ProjectPointAsync;
                    break;
                case "EXPORTMAPQUERYGEOMETRY":
                    action = ExportMapQueryGeometryRandomAsync;
                    break;
                case "EXPORTMAP":
                    action = ExportMapRandomAsync;
                    break;
                case "QUERYGEOMETRY":
                    action = QueryMapRandomAsync;
                    break;
                case "GETALLDETAILS":
                    action = InvokeRestAllDetailsAsync;
                    break;
                case "GETALLLAYERS":
                    action = InvokeRestLayerDetailsAsync;
                    break;
                case "GETFIND":
                    action = InvokeRestFindAsync;
                    break;
                case "GETIDENTIFY":
                    action = InvokeRestIdentifyAsync;
                    break;
                default:
                    break;
            }

            if (action != null)
            {
                CallByParallelInvoke(action);
            }

            _logger.Debug("total requests [ " + _totalRequests + " ]");
        }

        private void HitRestEndPointAsync()
        {
            HitRestServiceAsync("");
        }

        private void ProjectPointAsync()
        {
            HitRestServiceAsync("");
        }

        private void InvokeRestFindAsync()
        {
            //http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer/exts/ArcFMMapServer/id/34/locate?query=10503&returnAttributes=True&exactMatch=True&maxRecords=500&layerFields=OPERATINGNUMBER&spatialReference=%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d&f=json

            Random random = new Random();
            int randomNum = random.Next(73, 86);
            int randomOpNum = random.Next(1, 20503);
            string suffix = "/exts/ArcFMMapServer/id/";
            suffix += randomNum.ToString();
            suffix += "/locate?query=" + randomOpNum + "&returnAttributes=True&exactMatch=True&maxRecords=500&layerFields=OPERATINGNUMBER&spatialReference=%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d&f=json";
            HitRestServiceAsync(suffix + GetRandomSuffix());
        }
        private void InvokeRestIdentifyAsync()
        {
            //http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer/identify?geometryType=esriGeometryEnvelope&geometry=%7b%22xmin%22%3a2812950.3%2c%22ymin%22%3a12845621.6%2c%22xmax%22%3a2813368.82903226%2c%22ymax%22%3a12845905.883871%2c%22spatialReference%22%3a%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d%7d&returnGeometry=true&sr=%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d&imageDisplay=1218%2c620%2c96&layers=all%3a4%2c5%2c7%2c9%2c10%2c12%2c14%2c16%2c17%2c19%2c21%2c22%2c24%2c25%2c27%2c28%2c30%2c31%2c33%2c34%2c35%2c36%2c37%2c38%2c39%2c40%2c41%2c42%2c43%2c44%2c45%2c46%2c47%2c51%2c52%2c54%2c55%2c56%2c57%2c58%2c59%2c60%2c65%2c66%2c69%2c70%2c71%2c72%2c76&tolerance=5&mapExtent=2812715.1516129%2c12845404%2c2813783.8483871%2c128
            string suffix = "/identify?";
            suffix += "geometryType=esriGeometryEnvelope&geometry=%7b%22xmin%22%3a2812950.3%2c%22ymin%22%3a12845621.6%2c%22xmax%22%3a2813368.82903226%2c%22ymax%22%3a12845905.883871%2c%22spatialReference%22%3a%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d%7d&returnGeometry=true&sr=%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2c";
            suffix +=
                "DATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d&imageDisplay=1218%2c620%2c96&layers=all%3a4%2c5%2c7%2c9%2c10%2c12%2c14%2c16%2c17%2c19%2c21%2c22%2c24%2c25%2c27%2c28%2c30%2c31%2c33%2c34%2c35%2c36%2c37%2c38%2c39%2c40%2c41%2c42%2c43%2c44%2c45%2c46%2c47%2c51%2c52%2c54%2c55%2c56%2c57%2c58%2c59%2c60%2c65%2c66%2c69%2c70%2c71%2c72%2c76&tolerance=5&mapExtent=2812715.1516129%2c12845404%2c2813783.8483871%2c12845948&layerDefs=%7b%7d&f=json";
            HitRestServiceAsync(suffix + GetRandomSuffix());
        }

        private void InvokeRestAllDetailsAsync()
        {
//http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer?f=json&
//http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer/exts/ArcFMMapServer?f=json
//http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer/layers?f=json&
//http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer/legend?f=json&

            Random random = new Random();
            int randomNum = random.Next(1, 4);
            string suffix = "";
            switch (randomNum)
            {
                case 1:
                    suffix += "?f=json";
                    break;
                case 2:
                    suffix += "/exts/ArcFMMapServer?f=json";
                    break;
                case 3:
                    suffix += "/layers?f=json";
                    break;
                default:
                    suffix += "/legend?f=json";
                    break;
            }
            HitRestServiceAsync(suffix + GetRandomSuffix());
        }


        private void InvokeRestLayerDetailsAsync()
        {
//        http://wsgo496902:6080/arcgis/rest/services/Data/Schematics/MapServer/exts/ArcFMMapServer/id/33?f=json
            Random random = new Random();
            int randomNum = random.Next(0, 76);
            string suffix = "/exts/ArcFMMapServer/id/";
            suffix += randomNum.ToString();
            suffix += "?f=json";
            HitRestServiceAsync(suffix + GetRandomSuffix());
        }

        private void QueryMapRandomAsync()
        {
            //http://wsgo496902:6080/arcgis/rest/services/Data/PublicationFS/FeatureServer/2/query?where=&objectIds=&time=&geometry=%7B%22xmin%22%3A2811684.55283486%2C%22ymin%22%3A12845583.6292411%2C%22xmax%22%3A2812860.12535394%2C%22ymax%22%3A12845983.6292411%7D&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&gdbVersion=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&f=html

            string overrideServiceUrl = "http://wsgo496902:6080/arcgis/rest/services/Data/PublicationFS/FeatureServer";
            overrideServiceUrl = _serviceURL;

            string bbox = getBbox();
            string envelope = "{\"xmin\":" + bbox.Split(',')[0] + "," +
            "\"ymin\":" + bbox.Split(',')[1] + "," +
            "\"xmax\":" + bbox.Split(',')[2] + "," +
            "\"ymax\":" + bbox.Split(',')[3] + "}";

            for (int i = 0; i < 22; i++)
            {
                string queryString = "/" + i + "/query?where=&objectIds=&time=&geometry=";
                queryString += envelope;
                queryString +=
                    "&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&gdbVersion=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&f=json";
                HitRestServiceAsync(queryString, overrideServiceUrl);
                _totalRequests++;
            }

        }

        private void ExportMapQueryGeometryRandomAsync()
        {
            QueryMapRandomAsync();
            ExportMapRandomAsync();
        }

        private void ExportMapRandomAsync()
        {
            string queryString =
                GetRestServiceUrlParametersString(getBbox());

            HitRestServiceAsync("/export" + "?" + queryString);
            
        }


        private void CallByParallelInvoke(Action action)
        {

            Action[] actions = new Action[_jobCount];
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = new Action(action);
            }

            Parallel.Invoke(new ParallelOptions() {MaxDegreeOfParallelism = _threadCount}, actions);
        }

        private void CallByTaskList()
        {
            Task[] tasks = new Task[_threadCount];
            
            //TODO: chunk threads by some limit e.g. 50
            for (int i = 0; i < _threadCount; i++)
            {
                tasks[i++] = Task.Factory.StartNew(() =>
                {
                    ExportMapRandomAsync();
                });
            }
            while (tasks.Any(t => !t.IsCompleted))
            {
                Thread.Yield();
            } //spin wait

        }

        private string getBbox()
        {
            string bbox = "";

            const int width = 2813783 - 2812715;
            const int height = 12845948 - 12845404;

            int minX = Convert.ToInt32(BBOX_FRESNO.Split(',')[0]);
            int minY = Convert.ToInt32(BBOX_FRESNO.Split(',')[1]);
            int maxX = Convert.ToInt32(BBOX_FRESNO.Split(',')[2]);
            int maxY = Convert.ToInt32(BBOX_FRESNO.Split(',')[3]);
            Random rnd = new Random();

            int bboxMinX = rnd.Next(minX, maxX);
            int bboxMinY = rnd.Next(minY, maxY);
            int bboxMaxX = bboxMinX + width;
            int bboxMaxY = bboxMinY + height;

            bbox = bboxMinX + "," + bboxMinY + "," + bboxMaxX + "," + bboxMaxY;

            return bbox;
        }

        private string GetRestServiceUrlParametersString(string bbox)
        {
            string parameters = "";

            parameters += "bbox=" + bbox;
            parameters +=
                "&size=1218%2c620&dpi=96&format=png24&transparent=true&imageSR=%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d&bboxSR=%7b%22wkt%22%3a%22PROJCS%5b%5c%22NAD_1983_UTM_Zone_10N%5c%22%2cGEOGCS%5b%5c%22GCS_North_American_1983%5c%22%2cDATUM%5b%5c%22D_North_American_1983%5c%22%2cSPHEROID%5b%5c%22GRS_1980%5c%22%2c6378137.0%2c298.257222101%5d%5d%2cPRIMEM%5b%5c%22Greenwich%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Degree%5c%22%2c0.0174532925199433%5d%5d%2cPROJECTION%5b%5c%22Transverse_Mercator%5c%22%5d%2cPARAMETER%5b%5c%22False_Easting%5c%22%2c1640416.666666667%5d%2cPARAMETER%5b%5c%22False_Northing%5c%22%2c0.0%5d%2cPARAMETER%5b%5c%22Central_Meridian%5c%22%2c-123.0%5d%2cPARAMETER%5b%5c%22Scale_Factor%5c%22%2c0.9996%5d%2cPARAMETER%5b%5c%22Latitude_Of_Origin%5c%22%2c0.0%5d%2cUNIT%5b%5c%22Foot_US%5c%22%2c0.3048006096012192%5d%5d%22%7d&f=image";
            Random random = new Random();
            parameters += GetRandomSuffix();

            return parameters;
        }

        private string GetRandomSuffix()
        {
            Random random = new Random();
            return "&random=" + random.Next().ToString();
        }

        public void HitRestServiceAsync(string urlSuffix, string serviceURL = "")
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadDataCompleted += new DownloadDataCompletedEventHandler(wc_DownloadDataCompleted);
                wc.Headers["Content-type"] = "application/x-www-form-urlencoded";
                if (serviceURL == "")
                {
                    serviceURL = _serviceURL;
                }
                _logger.Debug("Hitting [ " + serviceURL + " ]");
                byte[] imageData = wc.DownloadData(new Uri(serviceURL + urlSuffix));

                if (imageData.Length < 1) throw new ArgumentException("blah");
                else _logger.Debug("Received response length [ " + imageData.Length + " ]");
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
            }
        }

        void wc_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            var x = 0;
        }

    }
}
