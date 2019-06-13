import React from "react";
import logo from "./logo.svg";
import Chat from "./chat/Chat";
import "./App.css";

const App: React.FC = () => {
  return (
    <div className="App">
      <Chat />
    </div>
  );
};

export default App;
