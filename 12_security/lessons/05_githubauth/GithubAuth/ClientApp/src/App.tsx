import React, { useState, useEffect } from "react";
import logo from "./logo.svg";
import "./App.css";
import * as api from "./services/apiClient";

function App() {
  let [isLoggedIn, setIsLoggedIn] = useState(false);
  let [userInfo, setUserInfo] = useState<api.IUserInfo>();

  useEffect(() => {
    let checkLogin = async () => {
      setIsLoggedIn(await api.isLoggedIn());
    };
    checkLogin();
  }, []);

  useEffect(() => {
    if (!isLoggedIn) {
      return;
    }

    let getUserInfo = async () => {
      let result = await api.getUserInfo();
      setUserInfo(result);
    };
    getUserInfo();
  }, [isLoggedIn]);

  return <div className="App">{isLoggedIn ? <UserInfoDisplay info={userInfo} /> : <a href="api/account/github/login">Login with GitHub</a>}</div>;
}

export default App;

const UserInfoDisplay: React.FC<{ info: api.IUserInfo | undefined }> = ({ info }) => {
  if (!info) {
    return <h1>Loading</h1>;
  }

  return (
    <ul>
      <li>Username: {info.login}</li>
    </ul>
  );
};
