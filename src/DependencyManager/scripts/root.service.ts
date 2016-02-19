import {Injectable} from 'angular2/core';
import {ROOTS} from './mock-roots';
import {Http} from 'angular2/http';
import {Root} from './root';

@Injectable()

export class RootService {

    roots: Root[] = [];
    root: Root;

    constructor(public http: Http) { }

    getRoots() {

        this.http.get('request/listRoots')
            .subscribe(
            data => {
                for (var i = 0; i < data.json().length; i++) {
                    this.root = { "type": data.json()[i][1], "name": data.json()[i][0], "feed": null };
                    this.roots.push(this.root);
                }
            },
            err => console.log(err)
        );

        return this.roots;
    }
}