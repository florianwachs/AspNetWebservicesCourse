import React, {useState} from "react";
import Axios from "axios";
import {Button, Card, Form, Icon, Input, Alert} from "antd";
import querystring from "querystring";
import {RouteComponentProps} from "@reach/router";
import SampleApiClient from "../shared/services/SampleApiClient";

const cardStyle = {width: 600, marginTop: "1rem"};
const headStyle = {backgroundColor: "#90b1ff"};

const ResourceOwnerPasswordFlowSample: React.FC<IResourceOwnerPasswordFlowSampleProps> = () => {
    const [token, setToken] = useState("");

    return (
        <div className="App">
            <Login onTokenSuccess={setToken}/>
            <TokenInfo token={token}/>
            <CallApi token={token}/>
        </div>
    );
};

export default ResourceOwnerPasswordFlowSample;

interface IResourceOwnerPasswordFlowSampleProps extends RouteComponentProps {
}

const CallApi: React.FC<{ token: string }> = ({token}) => {
    const [data, setData] = useState();

    const callApi = async () => {
        const result = await SampleApiClient.getForecasts(token);
        setData(JSON.stringify(result, null, 4));
    };

    return (
        <Card title="API aufrufen" style={cardStyle} headStyle={headStyle}>
            <div>
                <Button onClick={callApi}>Call Api</Button>
            </div>
            <div>
                <pre>{data}</pre>
            </div>
        </Card>
    );
};

const TokenInfo: React.FC<{ token: string }> = ({token}) => {

    return (
        <Card title="Token" style={cardStyle} headStyle={headStyle}>
            {token ? (<pre>{token}</pre>) : (
                <Alert message="Es wurde noch kein Token abgerufen" type="warning"/>)}
        </Card>

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

    return (<Card title="Token besorgen" style={cardStyle} headStyle={headStyle}>
            <Form onSubmit={handleSubmit} className="login-form">
                <Form.Item>
                    <Input prefix={<Icon type="user" style={{color: "rgba(0,0,0,.25)"}}/>} placeholder="Benutzername"
                           onChange={e => setUserName(e.target.value)}/>
                </Form.Item>
                <Form.Item>
                    <Input
                        prefix={<Icon type="lock" style={{color: "rgba(0,0,0,.25)"}}/>}
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
        </Card>

    );
};
