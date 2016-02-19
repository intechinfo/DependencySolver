System.register([], function(exports_1) {
    var Root;
    return {
        setters:[],
        execute: function() {
            Root = (function () {
                function Root(type, name, feed) {
                    this.type = type;
                    this.name = name;
                    this.feed = feed;
                }
                return Root;
            })();
            exports_1("Root", Root);
        }
    }
});
