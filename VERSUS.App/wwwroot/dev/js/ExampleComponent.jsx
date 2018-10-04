class ExampleComponent extends React.Component {

    componentDidMount() {
        rxjs.fromEvent(this.refs.click, "click")
        .subscribe(x => console.log(x));
    }

    render() {
        return (
            <div className="testDiv">
                <h1>{this.props.siteViewModel.announcement}</h1>
                <a ref="click">click works only once</a>
            </div>
        );
    }
}