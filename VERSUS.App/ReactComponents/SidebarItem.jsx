import { Component } from 'react';
import { Spring } from 'react-spring';

export default class SidebarItem extends Component {
    state = {
        hover: false,
        username: "",
        password: ""
    }

    handleChange = (event) => {
        this.setState({ [event.target.name]: event.target.value });
    }

    handleSubmit = (event) => {
        event.preventDefault();

        const request = {
            method: "POST",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },
            body: JSON.stringify({
                userName: this.state.username,
                userPassword: this.state.password
            })
        };

        fetch("/login", request)
            .then(response => {
                if (response.status !== 200) {
                    console.log(`Login response: ${response.status}`, response);
                    return;
                }
            })
            .catch(err => {
                console.log(`Login errored ${err.message}`, err);
            });
    }

    render() {
        const model = this.props.model;
        let url;

        if (model.icon) {
            url = model.icon.url;
        }

        let specialSidebarItem = null;

        switch (model.sidebarItemType) {
            case "link":
                specialSidebarItem = (
                    <a className="column" href={model.title} title={model.title}>
                        <img
                            src={url}
                        />
                    </a>
                );
                break;
            case "login":
                specialSidebarItem = (
                    <div className="column" >
                        <img
                            src={url}
                        />
                        <Spring
                            from={{
                                width: 0
                            }}
                            to={{
                                width: this.state.hover ? "auto" : 0
                            }}
                        >
                            {(styles) => (
                                <div className="loginForm" style={styles}>
                                    <form
                                        onSubmit={this.handleSubmit}
                                    >
                                        <label>
                                            Username:
                                            <input
                                                type="text"
                                                name="username"
                                                value={this.state.username}
                                                onChange={e => this.handleChange(e)}
                                            />
                                        </label>
                                        <label>
                                            Password:
                                            <input type="password"
                                                name="password"
                                                value={this.state.password}
                                                onChange={e => this.handleChange(e)}
                                            />
                                        </label>
                                        <input type="submit" value="Submit" />
                                    </form>
                                </div>
                            )}
                        </Spring>
                    </div>
                );
                break;
        }

        return (
            <div
                className={`row ${model.sidebarItemLocation}`}
                onMouseEnter={() => this.setState({ hover: true })}
                onMouseLeave={() => this.setState({ hover: false })}
            >
                {specialSidebarItem}
            </div>
        );
    }
}