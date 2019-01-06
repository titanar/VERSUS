import { Component } from 'react';
import ReactHtmlParser from 'react-html-parser';

export default class SidebarItem extends Component {
    render() {
        const model = this.props.model;
        let url;

        if (model.icon) {
            url = model.icon.url;
        }

        switch (model.sidebarItemLocation) {
            case "top":
            case "bottom":
                return (
                    <div className={`row ${model.sidebarItemLocation}`}>
                        <a className="column link" href={model.title} title={model.title}>
                            <img src={url} />
                        </a>
                    </div>
                );
            default:
                return (
                    <div className={`row ${model.sidebarItemLocation}`}></div>
                );
        };
    }
}