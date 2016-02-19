System.register(['angular2/core', 'angular2/router', './feed.Service'], function(exports_1) {
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, router_1, feed_Service_1;
    var FeedFormComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (feed_Service_1_1) {
                feed_Service_1 = feed_Service_1_1;
            }],
        execute: function() {
            FeedFormComponent = (function () {
                function FeedFormComponent(_feedService, router, params) {
                    this._feedService = _feedService;
                    this.params = params;
                    this.router = router;
                }
                FeedFormComponent.prototype.onSubmit = function (type, url, isPrivate, pwd, successDiv) {
                    this.newFeed = { "type": type.value, "url": url.value, "isPrivate": isPrivate.checked, "password": pwd.value };
                    this.FEEDS.push(this.newFeed);
                    var GetName = this.params.get('name');
                    this.router.navigate(['AddRoot', { name: GetName }]);
                };
                FeedFormComponent.prototype.IsPrivate = function (pwd) {
                    if (pwd.disabled) {
                        pwd.disabled = false;
                    }
                    else {
                        pwd.disabled = true;
                    }
                };
                FeedFormComponent.prototype.ngOnInit = function () {
                    this.FEEDS = this._feedService.getFeeds();
                };
                FeedFormComponent = __decorate([
                    core_1.Component({
                        selector: 'router-outlet'
                    }),
                    core_1.View({
                        templateUrl: './html/feed-form.component.html',
                        directives: [router_1.ROUTER_DIRECTIVES]
                    }), 
                    __metadata('design:paramtypes', [feed_Service_1.FeedService, router_1.Router, router_1.RouteParams])
                ], FeedFormComponent);
                return FeedFormComponent;
            })();
            exports_1("FeedFormComponent", FeedFormComponent);
        }
    }
});
