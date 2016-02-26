import {Component, OnInit} from 'angular2/core';
import {NgForm}    from 'angular2/common';
import {ROUTER_DIRECTIVES, Router, RouteParams} from 'angular2/router';
import {Root}    from './root';
import {Feed} from './feed.component';
import {RootService} from './root.Service';
import {FeedService} from './feed.Service';

@Component({
    selector: 'router-outlet',
    templateUrl: './html/root-form.component.html',
    directives: [ROUTER_DIRECTIVES]
})
export class RootFormComponent implements OnInit {
    ROOTS: Root[];
    newRoot: Root;
    FEEDS: Feed[];
    router: Router;
    selectedType: string;
    public GetName = this.params.get('name');

    constructor(private _rootService: RootService, private _feedService: FeedService, router: Router, private params: RouteParams) {
        this.router = router;
    }

    onSubmit(name: HTMLInputElement, type: HTMLInputElement, feed: HTMLInputElement, successDiv: HTMLDivElement) {
        this.newRoot = { "type": type.value, "name": name.value, "feed": feed.value }
        this.ROOTS.push(this.newRoot);

        name.value = "";
        type.value = "";
        feed.value = "";

        successDiv.hidden = false;
        setTimeout(function () {
            successDiv.hidden = true;
        }, 3000);
    }

    ToAddFeed(name: string, type: string) {
        this.router.navigate(['AddFeed', { name: name }]);
    }

    onselect(type: HTMLInputElement) {
        this.FEEDS = this._feedService.getSpecificFeeds(type.value);
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
        let GetType = this.params.get('type');
        let GetUrl = this.params.get('url');

        this.FillROOTS();
        this.FEEDS = this._feedService.getFeeds();
    }
}