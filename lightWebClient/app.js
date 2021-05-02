/**
 * On récupère toutes les stations à l'initialisation
 */
window.onload = function () {
    //retrieveAllStations();
};

// This variable will contain the list of the stations of the chosen contract.
var stations = [];

var pointsReq1;
var pointsReq2;
var pointsReq3;

var map;
var vectorOther;
var vector;

var feature1;
var feature;
var feature3;

var lineStyle;
var lineStyleOther;

var source;
var sourceOther;
var ptD;

map = new ol.Map({
    target: 'conteneur_map', // <-- This is the id of the div in which the map will be built.
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        })
    ],

    view: new ol.View({
        center: ol.proj.fromLonLat([7.0985774, 43.6365619]), // <-- Those are the GPS coordinates to center the map to.
        zoom: 13 // You can adjust the default zoom.
    })
});


function drawMap() {
    console.log(pointsReq2)

    // Create an array containing the GPS positions you want to draw

    var lineString_walkStart = new ol.geom.LineString(pointsReq1["coordinates"]);
    var lineString_Biking = new ol.geom.LineString(pointsReq2["coordinates"]);
    var lineString_walkEnd = new ol.geom.LineString(pointsReq3["coordinates"]);


    // Transform to EPSG:3857
    lineString_walkStart.transform('EPSG:4326', 'EPSG:3857');
    lineString_Biking.transform('EPSG:4326', 'EPSG:3857');
    lineString_walkEnd.transform('EPSG:4326', 'EPSG:3857');

    map.getView().setCenter(ol.proj.transform(pointsReq1["coordinates"][0], 'EPSG:4326', 'EPSG:3857'));

    map.getLayers().forEach(function(el) {
        console.log(el);
        if (el.get('name') === 'vectorun' || el.get('name') === 'vectordeux') {
            el.getSource().clear();
            console.log("11111");
        }
      })

    // Create the feature
    feature = new ol.Feature({
        geometry: lineString_Biking,
        name: 'Line'
    });

    feature1 = new ol.Feature({
        geometry: lineString_walkStart,
        name: 'Line'
    });

    feature3 = new ol.Feature({
        geometry: lineString_walkEnd,
        name: 'Line'
    });

    // Configure the style of the line
    lineStyle = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: '#ffcc33',
            width: 10
        })
    });

    lineStyleOther = new ol.style.Style({
        stroke: new ol.style.Stroke({
            color: '#FF0000',
            width: 10
        })
    });

    source = new ol.source.Vector({
        features: [feature]
    });

    sourceOther = new ol.source.Vector({
        features: [feature1, feature3]
    });
    

    vector = new ol.layer.Vector({
        source: source,
        style: [lineStyle]
    });

    vectorOther = new ol.layer.Vector({
        source: sourceOther,
        style: [lineStyleOther]
    });


    vector.set('name', 'vectorun');
    vectorOther.set('name', 'vectordeux');

    map.addLayer(vector);
    map.addLayer(vectorOther);
}


function callAPI(url, requestType, params, finishHandler) {
    var fullUrl = url;
    // If there are params, we need to add them to the URL.
    if (params.length > 0) {
        // Reminder: an URL looks like protocol://host?param1=value1&param2=value2 ...
        fullUrl += "?" + params.join("&");
    }

    // The js class used to call external servers is XMLHttpRequest.
    var caller = new XMLHttpRequest();
    caller.open(requestType, fullUrl, true);
    // The header set below limits the elements we are OK to retrieve from the server.
    caller.setRequestHeader("Accept", "application/json");
    // onload shall contain the function that will be called when the call is finished.

    caller.onload = finishHandler;
    caller.send();
}

function retrieveAllStations() {
    var targetUrl = "http://localhost:8722/Design_Time_Addresses/RoutingService/RoutingService/initialize";
    var requestType = "GET";
    var params = [];
    /* When the contracts are retrieved, we need to fill the contract list in Step2.
    ** This is done in the feedContractList function. */
    var onFinish = feedStationList;
    callAPI(targetUrl, requestType, params, onFinish);
}

function processRoute(e) {
    e.preventDefault();
    //Departure address
    var selectedStarting = document.getElementById("startingSelected").value;
    //Arrival address
    var selectedArrival = document.getElementById("arrivalSelected").value;

    var targetUrl = "http://localhost:8722/Design_Time_Addresses/RoutingService/RoutingService/process"
    // selectedStarting = selectedStarting.trim().replace(/ /g, '%20');
    // selectedArrival = selectedArrival.trim().replace(/ /g, '%20');
    var params = ["starting=" + selectedStarting, "arrival=" + selectedArrival];
    var requestType = "GET";

    var onFinish = findPointsList;
    callAPI(targetUrl, requestType, params, onFinish);
}



// This function is called when a XML call is finished. In this context, "this" refers to the API response.
function feedStationList() {
    // First of all, check that the call went through OK:
    if (this.status !== 200) {
        console.log("Stations not retrieved. Check the error in the Network or Console tab.");
    } else {
        // Let's fill the stations variable with the list we got from the API:
        var response = JSON.parse(this.responseText);
        // Then let's display the Step 3:
        console.log("response du routing --> " + response["initializeStationsResult"]);
        // document.getElementById("step3").style.display = "block";
    }
}

function findPointsList() {
    if (this.status !== 200) {
        console.log("Points not retrieved. Check the error in the Network or Console tab.");
    } else {

        //console.log(this.responseText);
        var result = JSON.parse(this.responseText);
        // var jsonData = result["searchRouteResult"][0];

        var request1Walking = JSON.parse(result["searchRouteResult"][0]);
        var request2Biking = JSON.parse(result["searchRouteResult"][1]);
        var request3Walking = JSON.parse(result["searchRouteResult"][2]);

        for (var k in request1Walking) {
            if (request1Walking[k] instanceof Object) {
                if (k === "features") {
                    pointsReq1 = request1Walking[k][0]["geometry"];
                    pointsReq2 = request2Biking[k][0]["geometry"];
                    pointsReq3 = request3Walking[k][0]["geometry"];
                }
            }
        }

        drawMap();
        //console.log(request2Biking["features"][0]["geometry"]["coordinates"]);



        // var request1Geometry = JSON.parse(request1Walking["features"][0])
        // console.log(request1Geometry)

        //console.log(jsonData[0]);

        // for (var i = 0; i < jsonData.geometry.length; i++) {
        //     var counter = jsonData.counters[i];
        //     console.log(counter.counter_name);
        // }
        // console.log(pp);
        // console.log(points["geometry"])
        //console.log("response du routing --> " + response["initializeStationsResult"]);
    }
}