System.register(['angular2/core', 'angular2/router', './roots.Component', './root.Service', './feed.Service', './root-form.component', './feed-form.component', './graph.component'], function(exports_1) {
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, router_1, roots_Component_1, root_Service_1, feed_Service_1, root_form_component_1, feed_form_component_1, graph_component_1;
    var AppComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (roots_Component_1_1) {
                roots_Component_1 = roots_Component_1_1;
            },
            function (root_Service_1_1) {
                root_Service_1 = root_Service_1_1;
            },
            function (feed_Service_1_1) {
                feed_Service_1 = feed_Service_1_1;
            },
            function (root_form_component_1_1) {
                root_form_component_1 = root_form_component_1_1;
            },
            function (feed_form_component_1_1) {
                feed_form_component_1 = feed_form_component_1_1;
            },
            function (graph_component_1_1) {
                graph_component_1 = graph_component_1_1;
            }],
        execute: function() {
            AppComponent = (function () {
                function AppComponent(_rootService, _feedService) {
                    this._rootService = _rootService;
                    this._feedService = _feedService;
                }
                AppComponent.prototype.ngOnInit = function () {
                    this.ROOTS = this._rootService.getRoots();
                    this.FEEDS = this._feedService.getFeeds();
                };
                AppComponent = __decorate([
                    core_1.Component({
                        selector: 'my-app',
                        templateUrl: './html/app.component.html',
                        directives: [router_1.ROUTER_DIRECTIVES],
                        providers: [root_Service_1.RootService, feed_Service_1.FeedService]
                    }),
                    router_1.RouteConfig([
                        { path: '/', name: 'Roots', component: roots_Component_1.RootsComponent, useAsDefault: true },
                        { path: '/AddRoot', name: 'AddRoot', component: root_form_component_1.RootFormComponent },
                        { path: '/AddFeed', name: 'AddFeed', component: feed_form_component_1.FeedFormComponent },
                        { path: '/Graph', name: 'Graph', component: graph_component_1.GraphComponent }
                    ]), 
                    __metadata('design:paramtypes', [root_Service_1.RootService, feed_Service_1.FeedService])
                ], AppComponent);
                return AppComponent;
            })();
            exports_1("AppComponent", AppComponent);
        }
    }
});
