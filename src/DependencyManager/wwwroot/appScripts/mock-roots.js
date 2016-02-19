System.register([], function(exports_1) {
    var ROOTS;
    return {
        setters:[],
        execute: function() {
            exports_1("ROOTS", ROOTS = [
                { "type": "NuGet", "name": "SimpleGitVersion.Cake", "feed": "google.fr" },
                { "type": "NuGet", "name": "CK.StObjec,Runtime", "feed": "google.fr" },
                { "type": "Git", "name": "Cofely", "feed": "google.fr" },
                { "type": "Git", "name": "HumanSide", "feed": "google.fr" },
                { "type": "NPM", "name": "NPMPackage", "feed": "google.fr" }
            ]);
        }
    }
});
