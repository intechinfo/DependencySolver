import {Component, OnInit, View} from 'angular2/core';
import {NgForm}    from 'angular2/common';
import {ROUTER_DIRECTIVES, Router, RouteParams} from 'angular2/router';
import {FeedService} from './feed.Service';
import {Feed} from './feed.component';

@Component({
    selector: 'router-outlet'
})
@View({
        templateUrl: './html/feed-form.component.html',
        directives: [ROUTER_DIRECTIVES]
})
export class FeedFormComponent implements OnInit {
    FEEDS: Feed[];
    newFeed: Feed;
    router: Router;

    constructor(private _feedService: FeedService, router: Router, private params: RouteParams) { this.router = router; }

    onSubmit(type: HTMLInputElement, url: HTMLInputElement, isPrivate: HTMLInputElement, pwd: HTMLInputElement, successDiv: HTMLDivElement) {
        this.newFeed = { "type": type.value, "url": url.value, "isPrivate": isPrivate.checked, "password": pwd.value }
        this.FEEDS.push(this.newFeed);

        let GetName = this.params.get('name');

        this.router.navigate(['AddRoot', { name: GetName }]);
    }

    IsPrivate(pwd: HTMLInputElement) {
        if (pwd.disabled) {
            pwd.disabled = false;
        } else {
            pwd.disabled = true;
        }
    }

    ngOnInit() {
        this.FEEDS = this._feedService.getFeeds();
    }
}