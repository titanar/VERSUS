import { Component } from 'react';
import AnnouncementItem from './AnnouncementItem.jsx';

export default class CookieConsentComponent extends Component {
    saveCookie() {
        console.log("Cookie string: " + this.props.model.cookieString);
    }

    render() {
        return (
            <AnnouncementItem model={this.props.model} closeAction={this.saveCookie.bind(this)} />
        );
    }
}