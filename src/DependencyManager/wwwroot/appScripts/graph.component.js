System.register(['angular2/core', 'angular2/router', 'angular2/http'], function(exports_1) {
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, router_1, http_1;
    var GraphComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            }],
        execute: function() {
            GraphComponent = (function () {
                function GraphComponent(_routeParams, http) {
                    this._routeParams = _routeParams;
                    this.http = http;
                }
                GraphComponent.prototype.ngOnInit = function () {
                    var _this = this;
                    this.root = this._routeParams.get('name');
                    this.http.get('request/RootPackage/' + this.root)
                        .toPromise()
                        .then(function (data) {
                        _this.Data = new DOMParser().parseFromString(data.text(), "text/xml");
                        _this.InitGraph(document.getElementsByClassName("cy")[0]);
                    });
                };
                GraphComponent.prototype.InitGraph = function (cy) {
                    if (typeof this.cyObj == 'undefined') {
                        cy.textContent = "";
                        cy.className = "cy";
                        this.cyObj = new cytoscape({
                            container: cy,
                            // initial viewport state:
                            zoom: 1,
                            pan: { x: 750, y: 375 },
                            style: cytoscape.stylesheet()
                                .selector('node')
                                .css({
                                'content': 'data(name)',
                                'width': '100px',
                                'height': '100px',
                                'text-valign': 'center',
                                'color': 'white',
                                'background-color': '#3DB2FF',
                                'text-outline-width': 1,
                                'text-outline-color': '#3DB2FF',
                                'text-wrap': 'wrap'
                            })
                                .selector('edge')
                                .css({
                                'target-arrow-shape': 'triangle',
                                'width': 3,
                                'line-color': '#7ECCFF',
                                'target-arrow-color': '#7ECCFF'
                            })
                                .selector('.base')
                                .css({
                                'width': '200px',
                                'height': '200px',
                                'shape': 'rectangle'
                            })
                                .selector('.platform')
                                .css({
                                'width': '75px',
                                'height': '75px',
                                'shape': 'triangle'
                            }),
                            // interaction options:
                            minZoom: 0.1,
                            maxZoom: 5,
                            zoomingEnabled: true,
                            userZoomingEnabled: true,
                            panningEnabled: true,
                            userPanningEnabled: true,
                            boxSelectionEnabled: false,
                            selectionType: 'single',
                            touchTapThreshold: 8,
                            desktopTapThreshold: 4,
                            autolock: false,
                            autoungrabify: false,
                            autounselectify: false,
                            // rendering options:
                            headless: false,
                            styleEnabled: true,
                            hideEdgesOnViewport: false,
                            hideLabelsOnViewport: false,
                            textureOnViewport: false,
                            motionBlur: false,
                            motionBlurOpacity: 0.2,
                            wheelSensitivity: 1,
                            pixelRatio: 'auto',
                        });
                        this.LoadGraph();
                    }
                };
                GraphComponent.prototype.LoadGraph = function () {
                    var racc = [this.Data.getElementsByTagName("VPackage")[0]
                            .getElementsByTagName("VPackageInfo")];
                    if (this.Data.getElementsByTagName("VPackage")[1] != undefined) {
                        racc.push(this.Data.getElementsByTagName("VPackage")[1]
                            .getElementsByTagName("VPackageInfo"));
                    }
                    var integralityDep = [];
                    for (var j = 0; j < racc.length; j++) {
                        var listVPack = [racc[j][0]];
                        var listDep = [];
                        for (var i = 0; i < listVPack.length; i++) {
                            if (this.cyObj.getElementById(listVPack[i].getAttribute('Id') + listVPack[i].getAttribute('Version')).length == 0) {
                                this.cyObj.add({
                                    group: "nodes",
                                    data: {
                                        id: listVPack[i].getAttribute('Id') + listVPack[i].getAttribute('Version'),
                                        name: listVPack[i].getAttribute("Id") + "\n" + listVPack[i].getAttribute('Version')
                                    },
                                    classes: "base"
                                });
                            }
                            if (i == listVPack.length - 1) {
                                listDep = listDep.concat(this.GraphAlgo(listVPack[i], listVPack[i].getAttribute('Id') + listVPack[i].getAttribute('Version')));
                                listVPack = listDep;
                            }
                            else {
                                listDep = listDep.concat(this.GraphAlgo(listVPack[i], listVPack[i].getAttribute('Id') + listVPack[i].getAttribute('Version')));
                            }
                        }
                        integralityDep = integralityDep.concat(listVPack);
                    }
                    this.UpdateVersion(integralityDep);
                    var opt = {
                        name: 'dagre',
                        // dagre algo options, uses default value on undefined
                        nodeSep: 10,
                        edgeSep: 10,
                        rankSep: 10,
                        rankDir: "LR",
                        minLen: function (edge) { return 1; },
                        edgeWeight: function (edge) { return 1; },
                        // general layout options
                        fit: true,
                        padding: 30,
                        animate: false,
                        animationDuration: 500,
                        animationEasing: undefined,
                        boundingBox: undefined,
                        ready: function () { },
                        stop: function () { } // on layoutstop
                    };
                    this.cyObj.layout(opt);
                };
                GraphComponent.prototype.GraphAlgo = function (racc, src) {
                    //for on each platform
                    for (var i = 0; i < racc.getElementsByTagName("Platform").length; i++) {
                        if (racc.getElementsByTagName("Platform")[i]
                            .getElementsByTagName("Dependency").length != 0) {
                            if (this.cyObj.getElementById(racc.getElementsByTagName("Platform")[i]
                                .getAttribute("Id").split(',')[0] +
                                racc.getElementsByTagName("Platform")[i]
                                    .getAttribute("Id").split('=')[1] + src).length == 0) {
                                this.cyObj.add({
                                    group: "nodes",
                                    data: {
                                        id: racc.getElementsByTagName("Platform")[i]
                                            .getAttribute("Id").split(',')[0] +
                                            racc.getElementsByTagName("Platform")[i]
                                                .getAttribute("Id").split('=')[1] + src,
                                        name: racc.getElementsByTagName("Platform")[i]
                                            .getAttribute("Id").split(',')[0] + "\n" +
                                            racc.getElementsByTagName("Platform")[i]
                                                .getAttribute("Id").split('=')[1]
                                    },
                                    classes: "platform"
                                });
                            }
                            if (this.cyObj.getElementById(src + racc.getElementsByTagName("Platform")[i]
                                .getAttribute("Id").split(',')[0] +
                                racc.getElementsByTagName("Platform")[i]
                                    .getAttribute("Id").split('=')[1] + src).length == 0) {
                                this.cyObj.add({
                                    group: "edges",
                                    data: {
                                        id: src + racc.getElementsByTagName("Platform")[i]
                                            .getAttribute("Id").split(',')[0] +
                                            racc.getElementsByTagName("Platform")[i]
                                                .getAttribute("Id").split('=')[1] + src,
                                        source: src,
                                        target: racc.getElementsByTagName("Platform")[i]
                                            .getAttribute("Id").split(',')[0] +
                                            racc.getElementsByTagName("Platform")[i]
                                                .getAttribute("Id").split('=')[1] + src
                                    }
                                });
                            }
                            //for on each dependency of the i platform
                            for (var j = 0; j < racc.getElementsByTagName("Platform")[i]
                                .getElementsByTagName("Dependency").length; j++) {
                                if (this.cyObj.getElementById(racc.getElementsByTagName("Platform")[i]
                                    .getElementsByTagName("Dependency")[j]
                                    .getAttribute("Id") +
                                    racc.getElementsByTagName("Platform")[i]
                                        .getElementsByTagName("Dependency")[j]
                                        .getAttribute("Version")).length == 0) {
                                    this.cyObj.add({
                                        group: "nodes",
                                        data: {
                                            id: racc.getElementsByTagName("Platform")[i]
                                                .getElementsByTagName("Dependency")[j]
                                                .getAttribute("Id") +
                                                racc.getElementsByTagName("Platform")[i]
                                                    .getElementsByTagName("Dependency")[j]
                                                    .getAttribute("Version"),
                                            name: racc.getElementsByTagName("Platform")[i]
                                                .getElementsByTagName("Dependency")[j]
                                                .getAttribute("Id") + "\nv" +
                                                racc.getElementsByTagName("Platform")[i]
                                                    .getElementsByTagName("Dependency")[j]
                                                    .getAttribute("Version")
                                        }
                                    });
                                }
                                if (this.cyObj.getElementById(racc.getElementsByTagName("Platform")[i]
                                    .getAttribute("Id").split(',')[0] +
                                    racc.getElementsByTagName("Platform")[i]
                                        .getAttribute("Id").split('=')[1] + src + racc.getElementsByTagName("Platform")[i]
                                    .getElementsByTagName("Dependency")[j]
                                    .getAttribute("Id") +
                                    racc.getElementsByTagName("Platform")[i]
                                        .getElementsByTagName("Dependency")[j]
                                        .getAttribute("Version")).length == 0) {
                                    this.cyObj.add({
                                        group: "edges",
                                        data: {
                                            id: racc.getElementsByTagName("Platform")[i]
                                                .getAttribute("Id").split(',')[0] +
                                                racc.getElementsByTagName("Platform")[i]
                                                    .getAttribute("Id").split('=')[1] + src + racc.getElementsByTagName("Platform")[i]
                                                .getElementsByTagName("Dependency")[j]
                                                .getAttribute("Id") +
                                                racc.getElementsByTagName("Platform")[i]
                                                    .getElementsByTagName("Dependency")[j]
                                                    .getAttribute("Version"),
                                            source: racc.getElementsByTagName("Platform")[i]
                                                .getAttribute("Id").split(',')[0] +
                                                racc.getElementsByTagName("Platform")[i]
                                                    .getAttribute("Id").split('=')[1] + src,
                                            target: racc.getElementsByTagName("Platform")[i]
                                                .getElementsByTagName("Dependency")[j]
                                                .getAttribute("Id") +
                                                racc.getElementsByTagName("Platform")[i]
                                                    .getElementsByTagName("Dependency")[j]
                                                    .getAttribute("Version")
                                        }
                                    });
                                }
                            }
                        }
                    }
                    return this.GetDep(racc.getElementsByTagName("Dependency"));
                };
                GraphComponent.prototype.GetDep = function (listElmnt) {
                    var nbr = 0;
                    var tempList = [];
                    var trouve;
                    for (var i = 0; i < listElmnt.length; i++) {
                        trouve = false;
                        for (var j = 0; j < tempList.length; j++) {
                            if (listElmnt[i].getAttribute('Id') + listElmnt[i].getAttribute('Version') === tempList[j].getAttribute('Id') + tempList[j].getAttribute('Version')) {
                                trouve = true;
                            }
                        }
                        if (trouve != true) {
                            tempList.push(listElmnt[i]);
                            nbr++;
                        }
                    }
                    var racc = this.Data.getElementsByTagName("VPackage")[0]
                        .getElementsByTagName("VPackageInfo");
                    for (var i = 0; i < tempList.length; i++) {
                        for (var j = 0; j < racc.length; j++) {
                            if (tempList[i].getAttribute('Id') + tempList[i].getAttribute('Version') == racc[j].getAttribute('Id') + racc[j].getAttribute('Version')) {
                                tempList[i] = racc[j];
                            }
                        }
                    }
                    return tempList;
                };
                GraphComponent.prototype.UpdateVersion = function (listDep) {
                    var newListDep = [];
                    for (var i = 0; i < listDep.length; i++) {
                        var trouve = false;
                        for (var j = 0; j < newListDep.length; j++) {
                            if (listDep[i].getAttribute('Id') == newListDep[j]["Value"]) {
                                trouve = true;
                            }
                        }
                        if (trouve == false) {
                            newListDep.push({ "PackageManager": listDep[i].getAttribute('PackageManager'), "Value": listDep[i].getAttribute('Id') });
                        }
                    }
                    var headers = new http_1.Headers({ 'Content-Type': 'application/json', 'Accept': 'text/xml' });
                    var options = new http_1.RequestOptions({ headers: headers });
                    this.http.post('request/ListVersions/', JSON.stringify(newListDep), options)
                        .toPromise()
                        .then(function (data) {
                    });
                };
                GraphComponent = __decorate([
                    core_1.Component({
                        selector: 'router-outlet',
                        templateUrl: './html/graph.component.html',
                        directives: [router_1.ROUTER_DIRECTIVES]
                    }), 
                    __metadata('design:paramtypes', [router_1.RouteParams, http_1.Http])
                ], GraphComponent);
                return GraphComponent;
            })();
            exports_1("GraphComponent", GraphComponent);
        }
    }
});
