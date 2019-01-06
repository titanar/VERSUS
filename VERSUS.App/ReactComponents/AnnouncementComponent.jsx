import { Component } from 'react';
import AnnouncementItem from './AnnouncementItem.jsx';

export default class AnnouncementComponent extends Component {
    render() {
        return (
            <div>
                {this.props.model.map(function (m, i) {
                    return (
                        <AnnouncementItem model={m} key={i} />
                    );
                })}
            </div>
        );
    }
}