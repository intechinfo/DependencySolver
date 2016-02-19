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
    ROOTS: Root[];
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

    ngOnInit() { this.ROOTS = this._rootService.getRoots(); }
}