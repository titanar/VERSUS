import { Spring } from 'react-spring';
import { fromEvent, from } from 'rxjs';
import { switchMap, throttleTime } from 'rxjs/operators';
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr';

export class SiteTest extends React.Component {
    constructor(props) {
        super(props);
        this.state = props;
    }

    componentDidMount() {
        // Find a way to move this to the window context or prevent it from being resolved in SSR
        var connection = new HubConnectionBuilder()
            .withUrl("/siteHub")
            .configureLogging(LogLevel.Information)
            .build();

        connection.start().catch(err => console.error(err));

        var clicked = fromEvent(this.refs.spring.refs.click, "click");

        clicked
            .pipe(throttleTime(1000))
            .pipe(switchMap(() => from(connection.invoke("ReturnSomething", { announcement: "test from client" }))))
            .subscribe(x =>
                this.setState({
                    announcement: x
                }, console.log(x))
            );
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
}