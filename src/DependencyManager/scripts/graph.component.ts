import {Component, OnInit} from 'angular2/core';
import {ROUTER_DIRECTIVES, RouteParams} from 'angular2/router';
import {Http} from 'angular2/http';

declare var cytoscape: any

@Component({
    selector: 'router-outlet',
    templateUrl: './html/graph.component.html',
    directives: [ROUTER_DIRECTIVES]
})

export class GraphComponent implements OnInit {

    cyObj: any
    Data: JSON
    public root: string

    constructor(private _routeParams: RouteParams, public http: Http) { }

    ngOnInit() {
        this.root = this._routeParams.get('name');

        this.http.get('request/RootPackage/' + this.root)
            .subscribe(
                data => console.log(data),
                err => console.log(err)
            );
    }

    LoadGraph(cy: HTMLDivElement) {

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
    }
}