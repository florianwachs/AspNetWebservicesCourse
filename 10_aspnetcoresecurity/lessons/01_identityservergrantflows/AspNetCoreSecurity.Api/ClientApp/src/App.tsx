import React, {useState} from "react";
import "antd/dist/antd.css";
import "./App.css";
import {Button, Input, Form, Icon} from "antd";
import Axios from "axios";
import querystring from "querystring";


const App: React.FC = () => {
    const [token, setToken] = useState("");
    console.log(token);

    return (
        <div className="App">
            {!token && (<Login onTokenSuccess={setToken}/>)}
            {token && <CallApi token={token}/>}
        </div>
    );
};

export default App;

const CallApi: React.FC<{ token: string }> = ({token}) => {
    const [data, setData] = useState();

    const callApi = async () => {
        const response = await Axios.get("https://localhost:44387/api/sampledata/weatherforecasts", {headers: {Authorization: "Bearer " + token}});
        setData(JSON.stringify(response.data));
    };

    return (
        <div>
            <div><Button onClick={callApi}>Call Api</Button></div>
            <div>
                {data}
            </div>
        </div>
    )
};

const Login: React.FC<{ onTokenSuccess: (token: string) => void }> = ({onTokenSuccess}) => {
    const [userName, setUserName] = useState();
    const [password, setPassword] = useState();

    async function tryGetAuthToken() {
        const query = querystring.stringify({
            grant_type: "password",
            username: userName,
            password: password,
            client_id: "legacy-js",
            scope: "api1"
        });
        const tokenEndpoint = "https://localhost:44386/connect/token";
        const response = await Axios.post(tokenEndpoint, query, {
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            }
        });
        const token = response.data.access_token;
        if (token) {
            onTokenSuccess(token);
        }
    }

    const handleSubmit = async (e: any) => {
        e.preventDefault();
        try {
            await tryGetAuthToken();
        } catch (e) {
            console.log("Failed");
            console.log(e);
        }
    };

    const canLogin = userName && password;

    return (
        <Form onSubmit={handleSubmit} className="login-form">
            <Form.Item>

                <Input
                    prefix={<Icon type="user" style={{color: 'rgba(0,0,0,.25)'}}/>}
                    placeholder="Benutzername"
                    onChange={e => setUserName(e.target.value)}
                />

            </Form.Item>
            <Form.Item>

                <Input
                    prefix={<Icon type="lock" style={{color: 'rgba(0,0,0,.25)'}}/>}
                    type="password"
                    placeholder="Passwort"
                    onChange={e => setPassword(e.target.value)}
                />

            </Form.Item>
            <Form.Item>

                <Button disabled={!canLogin} type="primary" htmlType="submit">
                    Anmelden
                </Button>
            </Form.Item>
        </Form>
    )
};
