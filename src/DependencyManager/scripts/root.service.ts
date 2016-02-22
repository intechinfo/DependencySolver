import {Injectable} from 'angular2/core';
import {ROOTS} from './mock-roots';
import {Http} from 'angular2/http';
import {Root} from './root';
import 'rxjs/Rx';

@Injectable()

export class RootService {

    roots: Root[] = [];
    root: Root;

    constructor(public http: Http) { }

    getRoots() {

        return this.http.get('request/listRoots').toPromise();
    }
}