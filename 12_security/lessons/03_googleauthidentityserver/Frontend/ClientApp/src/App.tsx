import React from "react";
import logo from "./logo.svg";
import "./App.css";
import AuthorizationCodeFlowSample from "./AuthorizationCodeFlowSample";

const App: React.FC = () => {
  return (
    <div className="App">
      <AuthorizationCodeFlowSample />
    </div>
  );
};

export default App;
