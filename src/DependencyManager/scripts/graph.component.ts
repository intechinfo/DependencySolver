import {Component, OnInit} from 'angular2/core';
import {ROUTER_DIRECTIVES, RouteParams} from 'angular2/router';
import {Http, Headers, RequestOptions} from 'angular2/http';

declare var cytoscape: any
declare var cydagre: any
declare var dagre: any

@Component({
    selector: 'router-outlet',
    templateUrl: './html/graph.component.html',
    directives: [ROUTER_DIRECTIVES]
})

export class GraphComponent implements OnInit {

    cyObj: any
    xmlDependencies: XMLDocument
    xmlLastVersions: XMLDocument
    public root: string
    Exceptions: Element[] = [];

    constructor(private _routeParams: RouteParams, public http: Http) { }

    //Initiation of the graph component
    ngOnInit() {
        this.root = this._routeParams.get('name');

        this.http.get('request/RootPackage/' + this.root)
            .toPromise()
            .then(data => {
                this.xmlDependencies = new DOMParser().parseFromString(data.text(), "text/xml");
                this.InitGraph(document.getElementsByClassName("cy")[0]);
            })
    }
    
    //Initiation of the graph object with options
    InitGraph(cy: Element) {

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
    }

    //Load roots and necessary stuff for graph
    LoadGraph() {

        var racc = [this.xmlDependencies.getElementsByTagName("VPackage")[0]
            .getElementsByTagName("VPackageInfo")];

        if (this.xmlDependencies.getElementsByTagName("VPackage")[1] != undefined) {
            racc.push(this.xmlDependencies.getElementsByTagName("VPackage")[1]
                .getElementsByTagName("VPackageInfo"));
        }

        var integralityDep: Element[] = [];

        for (var j = 0; j < racc.length; j++) {
            var listVPack = [racc[j][0]];
            var listDep: Element[] = [];

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

                initial.on('click', e => {
                    var actuallyHide = this.cyObj.elements().filter(function () {
                        return this.hasClass('hide');
                    });

                    if (actuallyHide.length == 0) {
                        var route = this.cyObj.elements().filter(function () {
                            return this.hasClass('outDated');
                        });

                        if (route.length != 0) {
                            var greenEle = this.cyObj.elements().filter(function () {
                                return !this.hasClass('outDated') && !this.hasClass('outDatedDepend');
                            });

                            greenEle.addClass('hide');
                        }
                    } else {
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
            nodeSep: 10, // the separation between adjacent nodes in the same rank
            edgeSep: 10, // the separation between adjacent edges in the same rank
            rankSep: 10, // the separation between adjacent nodes in the same rank
            rankDir: "LR", // 'TB' for top to bottom flow, 'LR' for left to right
            minLen: function (edge) { return 1; }, // number of ranks to keep between the source and target of the edge
            edgeWeight: function (edge) { return 1; }, // higher weight edges are generally made shorter and straighter than lower weight edges

            // general layout options
            fit: true, // whether to fit to viewport
            padding: 30, // fit padding
            animate: false, // whether to transition the node positions
            animationDuration: 500, // duration of animation in ms if enabled
            animationEasing: undefined, // easing of animation if enabled
            boundingBox: undefined, // constrain layout bounds; { x1, y1, x2, y2 } or { x1, y1, w, h }
            ready: function () { }, // on layoutready
            stop: function () { } // on layoutstop
        }

        this.cyObj.layout(opt);

    }

    //Algo to link root with platforms and dependencies
    GraphAlgo(racc: Element, src: string) {

        //for on each platform
        for (var i = 0; i < racc.getElementsByTagName("Platform").length; i++) {

            if (racc.getElementsByTagName("Platform")[i]
                .getElementsByTagName("Dependency").length != 0) {

                if (
                    this.cyObj.getElementById(
                        racc.getElementsByTagName("Platform")[i]
                            .getAttribute("Id").split(',')[0] +
                        racc.getElementsByTagName("Platform")[i]
                            .getAttribute("Id").split('=')[1] + src
                    ).length == 0
                ) {
                    
                    var infoId = racc.getElementsByTagName("Platform")[i]
                        .getAttribute("Id").split(',')[0] +
                        racc.getElementsByTagName("Platform")[i]
                            .getAttribute("Id").split('=')[1] + src;

                    var infoName = racc.getElementsByTagName("Platform")[i]
                        .getAttribute("Id");

                    if (infoName == "") {
                        infoName = "All platforms";
                    } else {
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

                if (
                    this.cyObj.getElementById(
                        src + racc.getElementsByTagName("Platform")[i]
                            .getAttribute("Id").split(',')[0] +
                        racc.getElementsByTagName("Platform")[i]
                            .getAttribute("Id").split('=')[1] + src
                    ).length == 0
                ) {

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

                    if (
                        this.cyObj.getElementById(
                            racc.getElementsByTagName("Platform")[i]
                                .getElementsByTagName("Dependency")[j]
                                .getAttribute("Id") +
                            racc.getElementsByTagName("Platform")[i]
                                .getElementsByTagName("Dependency")[j]
                                .getAttribute("Version")
                        ).length == 0
                    ) {
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

                    if (
                        this.cyObj.getElementById(
                            racc.getElementsByTagName("Platform")[i]
                                .getAttribute("Id").split(',')[0] +
                            racc.getElementsByTagName("Platform")[i]
                                .getAttribute("Id").split('=')[1] + src + racc.getElementsByTagName("Platform")[i]
                                    .getElementsByTagName("Dependency")[j]
                                    .getAttribute("Id") +
                            racc.getElementsByTagName("Platform")[i]
                                .getElementsByTagName("Dependency")[j]
                                .getAttribute("Version")
                        ).length == 0
                    ) {

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
    }

    //get list of given element without duplicates
    GetDep(listElmnt: NodeListOf<Element>) {
        var nbr = 0;
        var tempList: Element[] = [];
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
    }

    //Request versions and chechk each dependencies
    UpdateVersion(listDep: Element[]) {
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

        let headers = new Headers({ 'Content-Type': 'application/json', 'Accept': 'text/xml' });
        let options = new RequestOptions({ headers: headers });

        this.http.post('request/ListVersions/', JSON.stringify(newListDep), options)
            .toPromise()
            .then(data => {
                this.xmlLastVersions = new DOMParser().parseFromString(data.text(), "text/xml");
            })
            .then(x => {
                var racc = this.xmlLastVersions;
                this.cyObj.nodes()
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
                                        this.data("name", Name + "\n" + "(v" + newVersion + ")");

                                        return this;
                                    }
                                } else if (this.hasClass("prereleased")) {
                                    var newVersion = racc.getElementsByTagName('PackageLastVersion')[j].getElementsByTagName('PreRelease')[0].childNodes[0].nodeValue;

                                    if (!this.hasClass(newVersion)) {
                                        var Name = this.data("name");
                                        this.data("name", Name + "\n" + "(v" + newVersion + ")");

                                        return !this.hasClass(newVersion);
                                    }
                                }
                            }
                        }
                    })
                    .addClass('outDated');
            })
            .then(y => {
                let headers = new Headers({ 'Content-Type': 'application/json', 'Accept': 'text/xml' });
                let options = new RequestOptions({ headers: headers });
                let data = { "PackageManager": "Nuget", "Value": this.root };

                this.http.post('request/GetValidateNodes/', JSON.stringify(data), options)
                    .toPromise()
                    .then(data => {
                        var xmlValidateNodes = new DOMParser().parseFromString(data.text(), "text/xml");
                        var ValidateNodes = xmlValidateNodes.getElementsByTagName('ValidateNode');
                        for (var i = 0; i < ValidateNodes.length; i++) {

                            var elmnt = this.cyObj.getElementById(ValidateNodes[i].getAttribute('Id') + ValidateNodes[i].getAttribute('Version'));
                            this.Exceptions.push(elmnt);
                            elmnt.removeClass('outDated');
                            elmnt.addClass('outDatedNotified');
                            this.cyObj.elements().removeClass('outDatedDepend');
                        }

                        this.ChangeAndToggle();
                    })
            });
    }

    //Manage stuffs
    ChangeAndToggle() {

        var ElemtsOutDated = this.cyObj.elements().filter(function () {
            return this.hasClass('outDated') || this.hasClass('outDatedNotified')
        });

        ElemtsOutDated.on('click', e => {
            var elemts = this.cyObj.elements().filter(function () {
                return this.hasClass('hide');
            });

            if (elemts.length == 0) {
                let headers = new Headers({ 'Content-Type': 'application/json', 'Accept': 'text/xml' });
                let options = new RequestOptions({ headers: headers });
                var addOrRemove;

                if (this.Exceptions.indexOf(e.cyTarget[0]) == -1) {

                    this.Exceptions.push(e.cyTarget[0]);
                    e.cyTarget[0].removeClass('outDated');
                    e.cyTarget[0].addClass('outDatedNotified');
                    this.cyObj.elements().removeClass('outDatedDepend');

                    addOrRemove = "Add";
                } else {

                    e.cyTarget[0].removeClass('outDatedNotified');
                    e.cyTarget[0].addClass('outDated');
                    var idxEle = this.Exceptions.indexOf(e.cyTarget[0]);
                    this.Exceptions.splice(idxEle, 1);

                    addOrRemove = "Remove";
                }

                var data = { packageId: { "PackageManager": "Nuget", "Value": this.root }, vPackageId: { "PackageManager": "NuGet", "Id": e.cyTarget[0].data('package'), "Version": e.cyTarget[0].data('version') } };
                this.http.post("request/" + addOrRemove + "ValidateNodes", JSON.stringify(data), options)
                    .toPromise();

                ChangeColor(this.cyObj, this.Exceptions);
            }
        })

        ChangeColor(this.cyObj, this.Exceptions);

        function ChangeColor(cyObj, Exce) {
            var Elemnts = cyObj.elements().filter(function () {
                if (Exce != []) {
                    if (Exce.indexOf(this) == -1) {
                        return this.hasClass('outDated');
                    }
                } else {
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
                } else {
                    if (Package.hasClass('partialValidate')) {
                        Package.removeClass('partialValidate')
                    }
                }
            }
        }
    }
}