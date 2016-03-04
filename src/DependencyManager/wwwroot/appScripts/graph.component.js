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
                    this.Exceptions = [];
                }
                //Initiation of the graph component
                GraphComponent.prototype.ngOnInit = function () {
                    var _this = this;
                    this.root = this._routeParams.get('name');
                    this.http.get('request/RootPackage/' + this.root)
                        .toPromise()
                        .then(function (data) {
                        _this.xmlDependencies = new DOMParser().parseFromString(data.text(), "text/xml");
                        _this.InitGraph(document.getElementsByClassName("cy")[0]);
                    });
                };
                //Initiation of the graph object with options
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
                                'width': '150px',
                                'height': '150px',
                                'shape': 'rectangle'
                            })
                                .selector('.platform')
                                .css({
                                'width': '75px',
                                'height': '75px',
                                'shape': 'triangle'
                            })
                                .selector('.outDated')
                                .css({
                                'background-color': '#FF0000',
                                'text-outline-color': '#FF0000',
                                'line-color': '#FF0000',
                                'target-arrow-color': '#FF0000'
                            })
                                .selector('.outDatedDepend')
                                .css({
                                'background-color': '#FF0000',
                                'text-outline-color': '#FF0000',
                                'line-color': '#FF0000',
                                'target-arrow-color': '#FF0000'
                            })
                                .selector('.outDatedNotified')
                                .css({
                                'background-color': '#e89c03',
                                'text-outline-color': '#FFA148',
                            })
                                .selector('.partialValidate')
                                .css({
                                'background-color': '#ffffff',
                                'text-outline-color': '#ffffff',
                            })
                                .selector('.hide')
                                .css({
                                'display': 'none'
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
                //Load roots and necessary stuff for graph
                GraphComponent.prototype.LoadGraph = function () {
                    var _this = this;
                    var racc = [this.xmlDependencies.getElementsByTagName("VPackage")[0]
                            .getElementsByTagName("VPackageInfo")];
                    if (this.xmlDependencies.getElementsByTagName("VPackage")[1] != undefined) {
                        racc.push(this.xmlDependencies.getElementsByTagName("VPackage")[1]
                            .getElementsByTagName("VPackageInfo"));
                    }
                    var integralityDep = [];
                    for (var j = 0; j < racc.length; j++) {
                        var listVPack = [racc[j][0]];
                        var listDep = [];
                        if (this.cyObj.getElementById(listVPack[0].getAttribute('Id')).length == 0) {
                            this.cyObj.add({
                                group: "nodes",
                                data: {
                                    id: listVPack[0].getAttribute('Id'),
                                    name: listVPack[0].getAttribute("Id")
                                },
                                classes: "base initial"
                            });
                            var initial = this.cyObj.nodes().filter(function () {
                                return this.hasClass('initial');
                            });
                            initial.on('click', function (e) {
                                var actuallyHide = _this.cyObj.elements().filter(function () {
                                    return this.hasClass('hide');
                                });
                                if (actuallyHide.length == 0) {
                                    var route = _this.cyObj.elements().filter(function () {
                                        return this.hasClass('outDated');
                                    });
                                    if (route.length != 0) {
                                        var greenEle = _this.cyObj.elements().filter(function () {
                                            return !this.hasClass('outDated') && !this.hasClass('outDatedDepend');
                                        });
                                        greenEle.addClass('hide');
                                    }
                                }
                                else {
                                    actuallyHide.removeClass('hide');
                                }
                            });
                        }
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
                                this.cyObj.add({
                                    group: "edges",
                                    data: {
                                        id: listVPack[0].getAttribute('Id') + listVPack[i].getAttribute('Id') + listVPack[i].getAttribute('Version'),
                                        source: listVPack[0].getAttribute('Id'),
                                        target: listVPack[i].getAttribute('Id') + listVPack[i].getAttribute('Version')
                                    }
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
                //Algo to link root with platforms and dependencies
                GraphComponent.prototype.GraphAlgo = function (racc, src) {
                    //for on each platform
                    for (var i = 0; i < racc.getElementsByTagName("Platform").length; i++) {
                        if (racc.getElementsByTagName("Platform")[i]
                            .getElementsByTagName("Dependency").length != 0) {
                            if (this.cyObj.getElementById(racc.getElementsByTagName("Platform")[i]
                                .getAttribute("Id").split(',')[0] +
                                racc.getElementsByTagName("Platform")[i]
                                    .getAttribute("Id").split('=')[1] + src).length == 0) {
                                var infoId = racc.getElementsByTagName("Platform")[i]
                                    .getAttribute("Id").split(',')[0] +
                                    racc.getElementsByTagName("Platform")[i]
                                        .getAttribute("Id").split('=')[1] + src;
                                var infoName = racc.getElementsByTagName("Platform")[i]
                                    .getAttribute("Id");
                                if (infoName == "") {
                                    infoName = "All platforms";
                                }
                                else {
                                    infoName = infoName.split(',')[0] + "\n" +
                                        racc.getElementsByTagName("Platform")[i]
                                            .getAttribute("Id").split('=')[1];
                                }
                                this.cyObj.add({
                                    group: "nodes",
                                    data: {
                                        id: infoId,
                                        name: infoName
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
                                    var isRelease = "released";
                                    if (racc.getElementsByTagName("Platform")[i]
                                        .getElementsByTagName("Dependency")[j]
                                        .getAttribute("Version").includes("-")) {
                                        isRelease = "prereleased";
                                    }
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
                                                    .getAttribute("Version"),
                                            package: racc.getElementsByTagName("Platform")[i]
                                                .getElementsByTagName("Dependency")[j]
                                                .getAttribute("Id"),
                                            version: racc.getElementsByTagName("Platform")[i]
                                                .getElementsByTagName("Dependency")[j]
                                                .getAttribute("Version")
                                        },
                                        classes: racc.getElementsByTagName("Platform")[i]
                                            .getElementsByTagName("Dependency")[j]
                                            .getAttribute("Id") + " " +
                                            racc.getElementsByTagName("Platform")[i]
                                                .getElementsByTagName("Dependency")[j]
                                                .getAttribute("Version") + " " + isRelease
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
                //get list of given element without duplicates
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
                    var racc = this.xmlDependencies.getElementsByTagName("VPackage")[0]
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
                //Request versions and chechk each dependencies
                GraphComponent.prototype.UpdateVersion = function (listDep) {
                    var _this = this;
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
                        _this.xmlLastVersions = new DOMParser().parseFromString(data.text(), "text/xml");
                    })
                        .then(function (x) {
                        var racc = _this.xmlLastVersions;
                        _this.cyObj.nodes()
                            .filter(function () {
                            return !this.hasClass("platform") && !this.hasClass("base");
                        })
                            .filter(function () {
                            for (var j = 0; j < racc.getElementsByTagName('PackageLastVersion').length; j++) {
                                if (this.hasClass(racc.getElementsByTagName('PackageLastVersion')[j].getElementsByTagName('Id')[0].childNodes[0].nodeValue)) {
                                    if (this.hasClass("released")) {
                                        var newVersion = racc.getElementsByTagName('PackageLastVersion')[j].getElementsByTagName('Release')[0].childNodes[0].nodeValue;
                                        if (!this.hasClass(newVersion)) {
                                            var Name = this.data("name");
                                            this.data("name", Name + "\n" + "(" + newVersion + ")");
                                            return this;
                                        }
                                    }
                                    else if (this.hasClass("prereleased")) {
                                        var newVersion = racc.getElementsByTagName('PackageLastVersion')[j].getElementsByTagName('PreRelease')[0].childNodes[0].nodeValue;
                                        if (!this.hasClass(newVersion)) {
                                            var Name = this.data("name");
                                            this.data("name", Name + "\n" + "(" + newVersion + ")");
                                            return !this.hasClass(newVersion);
                                        }
                                    }
                                }
                            }
                        })
                            .addClass('outDated');
                    })
                        .then(function (y) {
                        var headers = new http_1.Headers({ 'Content-Type': 'application/json', 'Accept': 'text/xml' });
                        var options = new http_1.RequestOptions({ headers: headers });
                        var data = { "PackageManager": "Nuget", "Value": _this.root };
                        _this.http.post('request/GetValidateNodes/', JSON.stringify(data), options)
                            .toPromise()
                            .then(function (data) {
                            var xmlValidateNodes = new DOMParser().parseFromString(data.text(), "text/xml");
                            var ValidateNodes = xmlValidateNodes.getElementsByTagName('ValidateNode');
                            for (var i = 0; i < ValidateNodes.length; i++) {
                                var elmnt = _this.cyObj.getElementById(ValidateNodes[i].getAttribute('Id') + ValidateNodes[i].getAttribute('Version'));
                                _this.Exceptions.push(elmnt);
                                elmnt.removeClass('outDated');
                                elmnt.addClass('outDatedNotified');
                                _this.cyObj.elements().removeClass('outDatedDepend');
                            }
                            _this.ChangeAndToggle();
                        });
                    });
                };
                //Manage stuffs
                GraphComponent.prototype.ChangeAndToggle = function () {
                    var _this = this;
                    var ElemtsOutDated = this.cyObj.elements().filter(function () {
                        return this.hasClass('outDated') || this.hasClass('outDatedNotified');
                    });
                    ElemtsOutDated.on('click', function (e) {
                        var elemts = _this.cyObj.elements().filter(function () {
                            return this.hasClass('hide');
                        });
                        if (elemts.length == 0) {
                            var headers = new http_1.Headers({ 'Content-Type': 'application/json', 'Accept': 'text/xml' });
                            var options = new http_1.RequestOptions({ headers: headers });
                            var addOrRemove;
                            if (_this.Exceptions.indexOf(e.cyTarget[0]) == -1) {
                                _this.Exceptions.push(e.cyTarget[0]);
                                e.cyTarget[0].removeClass('outDated');
                                e.cyTarget[0].addClass('outDatedNotified');
                                _this.cyObj.elements().removeClass('outDatedDepend');
                                addOrRemove = "Add";
                            }
                            else {
                                e.cyTarget[0].removeClass('outDatedNotified');
                                e.cyTarget[0].addClass('outDated');
                                var idxEle = _this.Exceptions.indexOf(e.cyTarget[0]);
                                _this.Exceptions.splice(idxEle, 1);
                                addOrRemove = "Remove";
                            }
                            var data = { packageId: { "PackageManager": "Nuget", "Value": _this.root }, vPackageId: { "PackageManager": "NuGet", "Id": e.cyTarget[0].data('package'), "Version": e.cyTarget[0].data('version') } };
                            _this.http.post("request/" + addOrRemove + "ValidateNodes", JSON.stringify(data), options)
                                .toPromise();
                            ChangeColor(_this.cyObj, _this.Exceptions);
                        }
                    });
                    ChangeColor(this.cyObj, this.Exceptions);
                    function ChangeColor(cyObj, Exce) {
                        var Elemnts = cyObj.elements().filter(function () {
                            if (Exce != []) {
                                if (Exce.indexOf(this) == -1) {
                                    return this.hasClass('outDated');
                                }
                            }
                            else {
                                return this.hasClass('outDated');
                            }
                        });
                        for (var i = 0; i < Elemnts.length; i++) {
                            Recursive(Elemnts[i].incomers());
                        }
                        function Recursive(list) {
                            list.addClass('outDatedDepend');
                            var tree = list.incomers();
                            if (tree.length != 0) {
                                Recursive(tree);
                            }
                        }
                        var Package = cyObj.filter(function () {
                            return this.hasClass('initial');
                        })[0];
                        if (Package.hasClass('outDatedDepend') && Package.outgoers().nodes().length > 2) {
                            if (Package.outgoers().nodes()[0].hasClass('outDatedDepend') || Package.outgoers().nodes()[1].hasClass('outDatedDepend')) {
                                Package.addClass('partialValidate');
                            }
                            else {
                                if (Package.hasClass('partialValidate')) {
                                    Package.removeClass('partialValidate');
                                }
                            }
                        }
                    }
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
