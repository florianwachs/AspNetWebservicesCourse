import React, { useState } from "react";
import "./App.css";
import { Button } from "antd";
import Chat from "./chat/Chat";
import Weather from "./weather/Weather";

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
