import React, { useState } from "react";
import "antd/dist/antd.css";
import "./App.css";
import ResourceOwnerPasswordFlowSample from "./ResourceOwnerFlow/ResourceOwnerPasswordFlowSample";
import AuthorizationCodeFlowSample from "./AuthorizationCodeFlow/AuthorizationCodeFlowSample";
import { Menu, Icon } from "antd";
import { Router, Link } from "@reach/router";
import Home from "./Home";

const { SubMenu } = Menu;

const App: React.FC = () => {
  return (
    <div className="App">
      <Menu mode="horizontal">
        <SubMenu
          title={
            <span className="submenu-title-wrapper">
              <Icon type="setting" />
              Beispiele
            </span>
          }
        >
          <Menu.ItemGroup title="Autorisierung">
            <Menu.Item key="auth-with-password">
              <Link to="auth-with-password">Resource Owner Password Flow</Link>
            </Menu.Item>
            <Menu.Item key="auth-with-code">
              <Link to="auth-with-code">Authorization Code Flow</Link>
            </Menu.Item>
          </Menu.ItemGroup>
          <Menu.ItemGroup title="Item 2">
            <Menu.Item key="setting:3">Option 3</Menu.Item>
            <Menu.Item key="setting:4">Option 4</Menu.Item>
          </Menu.ItemGroup>
        </SubMenu>
      </Menu>
      <Router>
        <Home path="/" />
        <ResourceOwnerPasswordFlowSample path="auth-with-password" />
        <AuthorizationCodeFlowSample path="auth-with-code" />
      </Router>
    </div>
  );
};

export default App;
