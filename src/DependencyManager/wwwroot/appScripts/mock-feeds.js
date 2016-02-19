System.register([], function(exports_1) {
    var FEEDS;
    return {
        setters:[],
        execute: function() {
            exports_1("FEEDS", FEEDS = [
                { "type": "NuGet", "url": "google.fr", "isPrivate": true, "password": "1234" },
                { "type": "Git", "url": "yahoo.fr", "isPrivate": false, "password": "" },
                { "type": "NPM", "url": "orange.fr", "isPrivate": true, "password": "plop" },
            ]);
        }
    }
});
