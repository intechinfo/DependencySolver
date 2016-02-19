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
                    this.root = this._routeParams.get('name');
                    this.http.get('request/RootPackage/' + this.root)
                        .subscribe(function (data) { return console.log(data); }, function (err) { return console.log(err); });
                };
                GraphComponent.prototype.LoadGraph = function (cy) {
                    if (typeof this.cyObj == 'undefined') {
                        cy.textContent = "";
                        this.cyObj = new cytoscape({
                            container: cy,
                            // initial viewport state:
                            zoom: 1,
                            pan: { x: 0, y: 0 },
                            style: cytoscape.stylesheet()
                                .selector('node')
                                .css({
                                'content': 'data(name)',
                                'width': '50px',
                                'height': '50px',
                                'text-valign': 'center',
                                'color': 'white',
                                'background-color': '#3DB2FF',
                                'text-outline-width': 2,
                                'text-outline-color': '#3DB2FF'
                            })
                                .selector('edge')
                                .css({
                                'target-arrow-shape': 'triangle',
                                'width': 8,
                                'line-color': '#7ECCFF',
                                'target-arrow-color': '#7ECCFF'
                            }),
                            // interaction options:
                            minZoom: 0.5,
                            maxZoom: 5,
                            zoomingEnabled: true,
                            userZoomingEnabled: true,
                            panningEnabled: true,
                            userPanningEnabled: true,
                            boxSelectionEnabled: false,
                            selectionType: 'single',
                            touchTapThreshold: 8,
                            desktopTapThreshold: 4,
                            autolock: true,
                            autoungrabify: true,
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
                        this.cyObj.add({
                            group: "nodes",
                            data: { id: '1', name: this.Data },
                            position: { x: 50, y: 100 }
                        });
                        this.cyObj.add({
                            group: "nodes",
                            data: { id: '2', name: "Root of doom de l'apocalypse" },
                            position: { x: 200, y: 200 }
                        });
                        this.cyObj.add({
                            group: "edges",
                            data: { source: '1', target: '2' }
                        });
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
