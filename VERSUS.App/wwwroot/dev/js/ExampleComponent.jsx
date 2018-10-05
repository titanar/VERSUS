class ExampleComponent extends React.Component {

    render() {
        return (
            <div className="testDiv">
                <h1 ref="heading">{this.props.announcement}</h1>
                <a ref="click">click me</a>
            </div>
        );
    }

    componentDidMount() {
        var clicked = rxjs.fromEvent(this.refs.click, "click");
        
        clicked.pipe(
            rxjs.operators.mergeMap(x => this.refs.heading.textContent = "clicked!")
        ).pipe(
            rxjs.operators.mergeMap(x => rxjs.ajax.ajax("/test"))
        )
            .subscribe(x => console.log(x));
    }


}