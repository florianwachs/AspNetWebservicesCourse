import React, { useState } from "react";
import Chat from "./chat/Chat";
import "./App.css";
import Weather from "./weather/Weather";
import { Button } from "antd";

const App: React.FC = () => {
  const [showChat, setShowChat] = useState(true);

  return (
    <div className="App">
      <div>
        <Button onClick={() => setShowChat(true)}>Chat</Button>
        <Button onClick={() => setShowChat(false)}>Wetter</Button>
      </div>
      {showChat ? <Chat /> : <Weather />}
    </div>
  );
};

export default App;
