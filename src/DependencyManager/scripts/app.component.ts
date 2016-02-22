import {Component} from 'angular2/core';
import {RouteConfig, ROUTER_DIRECTIVES} from 'angular2/router';
import {RootsComponent} from './roots.Component';
import {RootService} from './root.Service';
import {FeedService} from './feed.Service';
import {Root} from './root.Component';
import {Feed} from './feed.Component';
import {RootFormComponent} from './root-form.component';
import {FeedFormComponent} from './feed-form.component';
import {GraphComponent} from './graph.component';

@Component({
    selector: 'my-app',
    templateUrl: './html/app.component.html',
    directives: [ROUTER_DIRECTIVES],
    providers: [RootService, FeedService]
})

@RouteConfig([
    { path: '/', name: 'Roots', component: RootsComponent, useAsDefault: true  },
    { path: '/AddRoot', name: 'AddRoot', component: RootFormComponent },
    { path: '/AddFeed', name: 'AddFeed', component: FeedFormComponent },
    { path: '/Graph', name: 'Graph', component: GraphComponent }
])

export class AppComponent {
}