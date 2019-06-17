import React, { useState } from "react";
import Axios from "axios";
import { Button, Form, Icon, Input } from "antd";
import querystring from "querystring";
import { RouteComponentProps } from "@reach/router";
import SampleApiClient from "../shared/services/SampleApiClient";
import { User, UserManager } from "oidc-client";

const AuthorizationCodeFlowSample: React.FC<IAuthorizationCodeFlowSampleProps> = () => {
  const [token, setToken] = useState("");

  return (
    <div className="App">
      {!token && <Login onTokenSuccess={setToken} />}
      {token && <CallApi token={token} />}
    </div>
  );
};

export default AuthorizationCodeFlowSample;
interface IAuthorizationCodeFlowSampleProps extends RouteComponentProps {}

const CallApi: React.FC<{ token: string }> = ({ token }) => {
  const [data, setData] = useState();

  const callApi = async () => {
    var result = await SampleApiClient.getForecasts(token);
    setData(JSON.stringify(result, null, 4));
  };

  return (
    <div>
      <div>
        <Button onClick={callApi}>Call Api</Button>
      </div>
      <div>
        <pre>{data}</pre>
      </div>
    </div>
  );
};

const Login: React.FC<{ onTokenSuccess: (token: string) => void }> = ({ onTokenSuccess }) => {
  return (
    <div>
      <Button type="primary" onClick={loginIfNeeded}>
        Login
      </Button>
    </div>
  );
};

const loginIfNeeded = async () => {
  const userManager = createUserManager();
  try {
    const user = await userManager.getUser();
    if (!user || user.expired) {
      login();
      return null;
    }
    console.log(user);
    return user;
  } catch (error) {
    console.log(error);
    login();
    return null;
  }
};

const login = () => {
  const userManager = createUserManager();
  userManager.signinRedirect({ state: window.location.href });
};

const createUserManager = () => {
  const config = {
    authority: "https://localhost:44386",
    client_id: "spa",
    redirect_uri: `https://localhost:44387/callback.html`,
    response_type: "code",
    scope: "openid profile api1",
    post_logout_redirect_uri: `https://localhost:44387/index.html`,
    automaticSilentRenew: true,
    silent_redirect_uri: `https://localhost:44387/silentrenew.html`
  };
  const um = new UserManager(config);
  um.events.addSilentRenewError(handleSilentRenewError);
  return um;
};
const handleSilentRenewError = (ev: any[]) => {
  console.log(ev);
};
