import React, { useState } from "react";
import "antd/dist/antd.css";
import "./App.css";
import ResourceOwnerPasswordFlowSample from "./ResourceOwnerFlow/ResourceOwnerPasswordFlowSample";
import { Menu, Icon } from "antd";
import { Router, Link } from "@reach/router";
import Home from "./Home";

const { SubMenu } = Menu;

const App: React.FC = () => {
  const handleClick = (e: any) => {
    console.log("click ", e);
  };

  return (
    <div className="App">
      <Menu onClick={handleClick} mode="horizontal">
        <SubMenu
          title={
            <span className="submenu-title-wrapper">
              <Icon type="setting" />
              Beispiele
            </span>
          }
        >
          <Menu.ItemGroup title="Autorisierung">
            <Menu.Item key="auth:1">
              <Link to="auth-1">Resource Owner Password Flow</Link>
            </Menu.Item>
            <Menu.Item key="auth:2">Authorization Code</Menu.Item>
          </Menu.ItemGroup>
          <Menu.ItemGroup title="Item 2">
            <Menu.Item key="setting:3">Option 3</Menu.Item>
            <Menu.Item key="setting:4">Option 4</Menu.Item>
          </Menu.ItemGroup>
        </SubMenu>
      </Menu>
      <Router>
        <Home path="/" />
        <ResourceOwnerPasswordFlowSample path="auth-1" />
      </Router>
    </div>
  );
};

export default App;
