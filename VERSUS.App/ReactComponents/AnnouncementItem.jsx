import { Component } from 'react';
import { Spring } from 'react-spring';
import ReactHtmlParser from 'react-html-parser';

export default class AnnouncementItem extends Component {
    buttonClass;
    buttonText;

    constructor(props) {
        super(props);

        switch (props.model.type) {
            case "close":
                this.buttonClass = "close";
                this.buttonText = "X";
                break;
            case "accept":
                this.buttonClass = "accept";
                this.buttonText = "Accept";
                break;
            default:
        }
    }

    state = {
        closed: false
    }

    close() {
        if (this.props.closeAction) {
            this.props.closeAction();
        }

        this.setState({
            closed: true
        })
    }

    render() {
        const model = this.props.model;
        const buttonClass = this.buttonClass;
        const buttonText = this.buttonText;

        return (
            <Spring
                from={{
                    height: "auto"
                }}
                to={{
                    height: this.state.closed ? 0 : "auto"
                }}
            >
                {(styles) =>
                    <div className="collapsible" style={styles}>
                        <div className={`row announcement ${model.location} ${model.level}`}>
                            <span className="h4 colonAfter">{model.title}</span>
                            <span className="h5 richText">{ReactHtmlParser(model.body[0].html)}</span>
                            <button
                                type="button"
                                className={`${buttonClass}`}
                                onClick={() => this.close()}
                            >{buttonText}</button>
                        </div>
                    </div>
                }
            </Spring>
        );
    }
}