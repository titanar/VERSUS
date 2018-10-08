fromEvent = rxjs.fromEvent;
mergeMap = rxjs.operators.mergeMap;
class ExampleComponent extends React.Component {
  render() {
    return React.createElement("div", {
      className: "testDiv"
    }, React.createElement("h1", {
      ref: "heading"
    }, this.props.announcement), React.createElement("a", {
      ref: "click"
    }, "click me"));
  }

  componentDidMount() {
    var clicked = fromEvent(this.refs.click, "click");
    clicked.pipe(mergeMap(x => this.refs.heading.textContent = "clicked!")).pipe(mergeMap(x => rxjs.ajax.ajax("/test"))).subscribe(x => console.log(x));
  }

}