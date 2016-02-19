System.register(['angular2/core', './mock-feeds'], function(exports_1) {
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, mock_feeds_1;
    var FeedService;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (mock_feeds_1_1) {
                mock_feeds_1 = mock_feeds_1_1;
            }],
        execute: function() {
            FeedService = (function () {
                function FeedService() {
                }
                FeedService.prototype.getFeeds = function () {
                    return mock_feeds_1.FEEDS;
                };
                FeedService.prototype.getSpecificFeeds = function (type) {
                    function FeedByType(elmt) {
                        return elmt.type === type;
                    }
                    return mock_feeds_1.FEEDS.filter(FeedByType);
                };
                FeedService = __decorate([
                    core_1.Injectable(), 
                    __metadata('design:paramtypes', [])
                ], FeedService);
                return FeedService;
            })();
            exports_1("FeedService", FeedService);
        }
    }
});
