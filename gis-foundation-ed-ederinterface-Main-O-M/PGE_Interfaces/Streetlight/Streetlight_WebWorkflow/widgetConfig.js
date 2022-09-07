define([
	"./widgets/layoutWidgets",
	"dojo/dom",
	"dojo/dom-class",
	"dojo/dom-style",
	"dojo/on",
	"dojo/query",
	"dojo/_base/array",
	"dojo/parser",
	"dojo/ready"],
			function (
                layoutWidgets, dom, domClass, domStyle, on, query, array,
                parser, ready) {
			    ready(function () {
			        var layoutWidgetsContainer = null;
			        parser.parse();
			        layoutWidgetsContainer = new layoutWidgets(null, "layoutWidgetsContent");


			        

			    });

			});