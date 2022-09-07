var map;
require(["esri/map",
         "esri/tasks/query",
         "esri/tasks/QueryTask",
         "esri/geometry/Extent",
         "esri/layers/ArcGISDynamicMapServiceLayer",
         "esri/layers/ArcGISTiledMapServiceLayer",
         "esri/geometry/webMercatorUtils",
         "esri/tasks/GeometryService",
         "esri/tasks/ProjectParameters",
         "esri/SpatialReference",
         "esri/layers/FeatureLayer",
         "esri/geometry/Point",
         "esri/graphic",
         "esri/Color",
         "esri/symbols/SimpleFillSymbol",
         "esri/symbols/SimpleLineSymbol",
         "esri/symbols/SimpleMarkerSymbol",
         "dojo/on",
         "dijit/registry",
         "dojo/parser",
         "dojox/widget/Standby",
         "dojo/domReady!"], function (Map, Query, QueryTask, Extent, ArcGISDynamicMapServiceLayer, ArcGISTiledMapServiceLayer, webMercatorUtils, GeometryService, ProjectParameters, SpatialReference, FeatureLayer, Point, Graphic, Color, SimpleFillSymbol, SimpleLineSymbol, SimpleMarkerSymbol, on, registry, parser, Standby) {

             parser.parse();

             var sapEquipId = getParameterFromUrl("SAPEQUIPID", "");
             var latitude = getParameterFromUrl("LAT", "");
             var longitude = getParameterFromUrl("LONG", "");

             var pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_CIRCLE, 50, new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0]), 2), new Color([255, 0, 0, 0.25]));

             map = new Map("mapDiv");
             
            
             
             loadLayers();
             on(map, "load", searchPole);
            
             function searchPole() {

                 
                 if (sapEquipId != null && latitude == null && longitude == null) {
                     var poleQueryTask = new QueryTask(config.PoleQueryLayer.Url);
                     var poleQuery = new Query();
                     poleQuery.returnGeometry = true;
                     poleQuery.outFields = ["*"];
                     poleQuery.where = "SAPEQUIPID='" + sapEquipId + "'";
                     poleQueryTask.execute(poleQuery, showResults, queryFailed);
                 } else if (sapEquipId == null && latitude != null && longitude != null && latitude != "" && longitude != "") {

                     zoomtoLocation(latitude, longitude);
                 }

                 else {
                     alert("Please pass SAP EQUIP ID or Lat/Long");
                 }
             }


             function zoomtoLocation(lat, long) {

                 var pointXY = webMercatorUtils.lngLatToXY(lat, long);

                 var geometryService = new esri.tasks.GeometryService(config.GeometryService.Url);
                 var inputpoint = new esri.geometry.Point(long, lat, new SpatialReference(4326));
                 var PrjParams = new esri.tasks.ProjectParameters();
                 PrjParams.geometries = [inputpoint];
                 PrjParams.outSR = map.spatialReference;
                 var wkid = map.spatialReference.wkid;
                
                 geometryService.project(PrjParams, outputpoint, errorFun);




             }

             function outputpoint(output) {
                 var tempPoint = new Point(output[0].x, output[0].y, map.spatialReference);
                 var graphic = new Graphic(tempPoint, pointSymbol);
                 map.graphics.add(graphic);
                 map.setScale(700);
                 map.centerAt(tempPoint);

             }

             function errorFun(e) {
                 console.log("Error");
             }
             function queryFailed(e) {
                 console.log("Error");
             }

             function showResults(featureset) {
                 map.graphics.clear();
                 var resultCount = featureset.features.length;
                 if (resultCount > 0) {
                     var feature = featureset.features[0];
                     var graphic = new Graphic(feature.geometry, pointSymbol);
                     map.graphics.add(graphic);

                     map.setScale(700);
                     map.centerAt(feature.geometry);
                    
                 } else {

                     alert("No pole found for SAPEQUIPID: " + sapEquipId);
                 }
             }

             

             function loadLayers() {
                 
                 var layerList = config.Layers;
                 var layersArray = [];
                 if (layerList.length > 0) {
                     for (var i = 0; i < layerList.length; i++) {
                         if (layerList[i].Visible == "True") {
                             if (layerList[i].Type == "ArcGISDynamicMapServiceLayer") {
                                 var layer = new ArcGISDynamicMapServiceLayer(layerList[i].Url);
                                 layer.id = layerList[i].MapServiceName;
                                 if (layerList[i].MapServiceName == "Electric Distribution") {
                                     layer.setVisibleLayers(layerList[i].VisibleLayerIds);
                                 }
                                 layersArray.push(layer);
                             } else if (layerList[i].Type == "FeatureLayer") {
                                 var layer = new FeatureLayer(layerList[i].Url);
                                 layer.id = layerList[i].MapServiceName;
                                 layersArray.push(layer);
                             } else if (layerList[i].Type == "ArcGISTiledMapServiceLayer") {
                                 var layer = new ArcGISTiledMapServiceLayer(layerList[i].Url);
                                 layer.id = layerList[i].MapServiceName;
                                 layersArray.push(layer);
                             }
                         }
                     }
                     map.addLayers(layersArray);

                 }
             }


             function getParameterFromUrl(name, url) {
                 if (!url) {
                     url = window.location.href;
                 }
                 name = name.replace(/[\[\]]/g, "\\$&");
                 var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                 results = regex.exec(url);
                 if (!results) return null;
                 if (!results[2]) return '';
                 return decodeURIComponent(results[2].replace(/\+/g, " "));
             }



         });