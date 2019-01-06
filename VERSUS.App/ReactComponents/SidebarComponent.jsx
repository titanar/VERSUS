import { Component } from 'react';
import SidebarItem from './SidebarItem.jsx';

export default class SidebarComponent extends Component {
    render() {
        return (
            <div className="column">
                {this.props.model.map(function (m, i) {
                    return (
                        <SidebarItem model={m} key={i} />
                    );
                })}
            </div>
        );
    }
}