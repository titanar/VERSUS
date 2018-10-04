// This is our component.
class ExampleComponent extends React.Component {
    render() {
        return (
            <div className="testDiv">
                <h1>Url: {this.props.Announcement}</h1>
            </div>
        );
    }
}