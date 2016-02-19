import {Injectable} from 'angular2/core';
import {Feed} from './feed.component/';
import {FEEDS} from './mock-feeds';

@Injectable()

export class FeedService {
    getFeeds() {
        return FEEDS;
    }

    getSpecificFeeds(type: string) {

        function FeedByType(elmt: Feed) {
            return elmt.type === type;
        }

        return FEEDS.filter(FeedByType);
    }
}