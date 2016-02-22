import {Component, OnInit} from 'angular2/core';
import {Root} from './root.Component';
import {Http, HTTP_PROVIDERS} from 'angular2/http';
import {ROUTER_DIRECTIVES, Router} from 'angular2/router';
import {Location} from 'angular2/router';
import {RootService} from './root.Service';

@Component({
    selector: 'router-outlet',
    templateUrl: './html/roots.component.html',
    viewProviders: [HTTP_PROVIDERS],
    directives: [ROUTER_DIRECTIVES]
})

export class RootsComponent implements OnInit {
    ROOTS: Root[] = [];
    public SelectRoot = "";

    constructor(private _rootService: RootService, private _router: Router, public http: Http) { }

    ConfSelect(root: string, conf: HTMLDivElement, tab: HTMLTableElement)
    {
        this.SelectRoot = root;

        conf.hidden = false;
        tab.hidden = true;
    }

    OpenGraph(root: Root) {
        this._router.navigate(['Graph', { name: root.name }]);
    }

    DelSelect(resp: boolean, conf: HTMLDivElement, tab: HTMLTableElement)
    {
        if (resp) {
            //var indx = this.ROOTS.map(function (e) { return e.name }).indexOf(this.SelectRoot);
            //this.ROOTS.splice(indx, 1);
        }

        conf.hidden = true;
        tab.hidden = false;
    }

    FillROOTS() {
        this._rootService.getRoots().then(data => {
            var root: Root
            for (var i = 0; i < data.json().length; i++) {
                root = { "type": data.json()[i][1], "name": data.json()[i][0], "feed": null };

                if (data.json()[i][0] === "System.Collections") {
                    this.ROOTS.push(root);
                }
            }
        })
    }

    ngOnInit() {
        this.FillROOTS();
    }
}