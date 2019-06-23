import React, { useState, useEffect } from "react";
import { Alert, Button, Card, Form, Icon, Input } from "antd";
import { User, UserManager } from "oidc-client";
import Axios from "axios";

const cardStyle = { width: 600, marginTop: "1rem" };
const headStyle = { backgroundColor: "#90b1ff" };

const AuthorizationCodeFlowSample: React.FC<IAuthorizationCodeFlowSampleProps> = () => {
  const [user, setUser] = useState<User>();

  useEffect(() => {
    const checkUser = async () => {
      const userManager = createUserManager();
      try {
        const user = await userManager.getUser();

        if (user && !user.expired) {
          setUser(user);
        }
      } catch (error) {
        console.log(error);
      }
    };

    checkUser();
  }, []);

  return (
    <div className="App">
      <Login />
      <UserInfo user={user} />
      <CallApi token={user && user.access_token} />
    </div>
  );
};

export default AuthorizationCodeFlowSample;

interface IAuthorizationCodeFlowSampleProps {}

const UserInfo: React.FC<{ user?: User }> = ({ user }) => {
  return (
    <Card title="User-Info" style={cardStyle} headStyle={headStyle}>
      {user ? <pre>{JSON.stringify(user, null, 4)}</pre> : <Alert message="Keine Authorisierung vorhanden" />}
    </Card>
  );
};

const CallApi: React.FC<{ token: string | undefined }> = ({ token }) => {
  const [data, setData] = useState();

  const callApi = async () => {
    const result = await Axios.get("api/sampledata/weatherforecasts", {
      headers: {
        Authorization: "Bearer " + token
      }
    });
    setData(JSON.stringify(result.data, null, 4));
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

const Login: React.FC = () => {
  return (
    <Card title={"Login"} style={cardStyle} headStyle={headStyle}>
      <Button type="primary" onClick={loginIfNeeded}>
        Login
      </Button>
    </Card>
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
    authority: "https://localhost:44318",
    client_id: "spa",
    redirect_uri: `https://localhost:44356/callback.html`,
    response_type: "code",
    scope: "openid profile api1",
    post_logout_redirect_uri: `https://localhost:44356/index.html`,
    automaticSilentRenew: true,
    silent_redirect_uri: `https://localhost:44356/silentrenew.html`
  };
  const um = new UserManager(config);
  um.events.addSilentRenewError(handleSilentRenewError);
  return um;
};
const handleSilentRenewError = (ev: any[]) => {
  console.log(ev);
};
