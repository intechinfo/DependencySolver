System.register(['angular2/core', 'angular2/router', './root.Service', './feed.Service'], function(exports_1) {
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, router_1, root_Service_1, feed_Service_1;
    var RootFormComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (root_Service_1_1) {
                root_Service_1 = root_Service_1_1;
            },
            function (feed_Service_1_1) {
                feed_Service_1 = feed_Service_1_1;
            }],
        execute: function() {
            RootFormComponent = (function () {
                function RootFormComponent(_rootService, _feedService, router, params) {
                    this._rootService = _rootService;
                    this._feedService = _feedService;
                    this.params = params;
                    this.GetName = this.params.get('name');
                    this.router = router;
                }
                RootFormComponent.prototype.onSubmit = function (name, type, feed, successDiv) {
                    this.newRoot = { "type": type.value, "name": name.value, "feed": feed.value };
                    this.ROOTS.push(this.newRoot);
                    name.value = "";
                    type.value = "";
                    feed.value = "";
                    successDiv.hidden = false;
                    setTimeout(function () {
                        successDiv.hidden = true;
                    }, 3000);
                };
                RootFormComponent.prototype.ToAddFeed = function (name, type) {
                    this.router.navigate(['AddFeed', { name: name }]);
                };
                RootFormComponent.prototype.onselect = function (type) {
                    this.FEEDS = this._feedService.getSpecificFeeds(type.value);
                };
                RootFormComponent.prototype.ngOnInit = function () {
                    var GetType = this.params.get('type');
                    var GetUrl = this.params.get('url');
                    //this.ROOTS = this._rootService.getRoots();
                    this.FEEDS = this._feedService.getFeeds();
                };
                RootFormComponent = __decorate([
                    core_1.Component({
                        selector: 'router-outlet',
                        templateUrl: './html/root-form.component.html',
                        directives: [router_1.ROUTER_DIRECTIVES]
                    }), 
                    __metadata('design:paramtypes', [root_Service_1.RootService, feed_Service_1.FeedService, router_1.Router, router_1.RouteParams])
                ], RootFormComponent);
                return RootFormComponent;
            })();
            exports_1("RootFormComponent", RootFormComponent);
        }
    }
});
