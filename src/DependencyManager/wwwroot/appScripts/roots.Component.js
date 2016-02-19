System.register(['angular2/core', 'angular2/http', 'angular2/router', './root.Service'], function(exports_1) {
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, http_1, router_1, root_Service_1;
    var RootsComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (root_Service_1_1) {
                root_Service_1 = root_Service_1_1;
            }],
        execute: function() {
            RootsComponent = (function () {
                function RootsComponent(_rootService, _router, http) {
                    this._rootService = _rootService;
                    this._router = _router;
                    this.http = http;
                    this.SelectRoot = "";
                }
                RootsComponent.prototype.ConfSelect = function (root, conf, tab) {
                    this.SelectRoot = root;
                    conf.hidden = false;
                    tab.hidden = true;
                };
                RootsComponent.prototype.OpenGraph = function (root) {
                    this._router.navigate(['Graph', { name: root.name }]);
                };
                RootsComponent.prototype.DelSelect = function (resp, conf, tab) {
                    if (resp) {
                    }
                    conf.hidden = true;
                    tab.hidden = false;
                };
                RootsComponent.prototype.ngOnInit = function () { this.ROOTS = this._rootService.getRoots(); };
                RootsComponent = __decorate([
                    core_1.Component({
                        selector: 'router-outlet',
                        templateUrl: './html/roots.component.html',
                        viewProviders: [http_1.HTTP_PROVIDERS],
                        directives: [router_1.ROUTER_DIRECTIVES]
                    }), 
                    __metadata('design:paramtypes', [root_Service_1.RootService, router_1.Router, http_1.Http])
                ], RootsComponent);
                return RootsComponent;
            })();
            exports_1("RootsComponent", RootsComponent);
        }
    }
});
