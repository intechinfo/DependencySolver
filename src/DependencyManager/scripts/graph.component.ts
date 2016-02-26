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
    Data: XMLDocument
    public root: string

    constructor(private _routeParams: RouteParams, public http: Http) { }

    ngOnInit() {
        this.root = this._routeParams.get('name');

        this.http.get('request/RootPackage/' + this.root)
            .toPromise()
            .then(data => {
                this.Data = new DOMParser().parseFromString(data.text(), "text/xml");
                this.InitGraph(document.getElementsByClassName("cy")[0]);
            })
    }
    
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
    }

    LoadGraph() {

        var racc = [this.Data.getElementsByTagName("VPackage")[0]
            .getElementsByTagName("VPackageInfo")];

        if (this.Data.getElementsByTagName("VPackage")[1] != undefined) {
            racc.push(this.Data.getElementsByTagName("VPackage")[1]
                .getElementsByTagName("VPackageInfo"));
        }

        var integralityDep: Element[] = [];

        for (var j = 0; j < racc.length; j++) {
            var listVPack = [racc[j][0]];
            var listDep: Element[] = [];

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
    }

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
            })
    }
}