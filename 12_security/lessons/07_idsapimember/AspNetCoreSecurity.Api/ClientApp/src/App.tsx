import React, { useState } from "react";
import "antd/dist/antd.css";
import "./App.css";
import AuthorizationCodeFlowSample from "./AuthorizationCodeFlow/AuthorizationCodeFlowSample";
import { Menu } from "antd";
import { BrowserRouter as Router, Route, Switch, Redirect, Link } from "react-router-dom";
import Home from "./Home";

const { SubMenu } = Menu;

const App: React.FC = () => {
  return (
    <Router>
      <div className="App">
        <Menu mode="horizontal">
          <SubMenu title={<span className="submenu-title-wrapper">Beispiele</span>}>
            <Menu.ItemGroup title="Autorisierung">
              <Menu.Item key="auth-with-code">
                <Link to="auth-with-code">Authorization Code Flow</Link>
              </Menu.Item>
            </Menu.ItemGroup>
          </SubMenu>
        </Menu>
        <Route path="/" component={Home} />
        <Route path="/auth-with-code" component={AuthorizationCodeFlowSample} />
      </div>
    </Router>
  );
};

export default App;
