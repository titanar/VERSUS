import { Spring } from 'react-spring';
import { fromEvent } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

export class SiteTest extends React.Component {

    constructor(props) {
        super(props);
        this.state = props;
    }

    render() {
        return (
            <h1
                style={{
                    lineHeight: '2',
                    color: '#222',
                    fontFamily: 'Helvetica, sans-serif',
                    textShadow: '0 0 5px lightgray',
                }}
            >
                <Spring from={{ opacity: 0 }} to={{ opacity: 1 }} ref="spring">
                    {styles =>
                        <div>
                            <a style={styles} ref="click">fade in, click me</a>
                            <h1 ref="heading">{this.state.announcement}</h1>
                        </div>
                    }
                </Spring>
			</h1>
        );
    }

    componentDidMount() {
        var clicked = fromEvent(this.refs.spring.refs.click, "click");

        clicked
            //.pipe(mergeMap(x => this.refs.heading.textContent = "clicked!"))
            .pipe(mergeMap(x => fetch("/test")))
            .subscribe(x =>
                this.setState({
                    announcement: "clicked!"
                }, console.log(x)));
    }
}