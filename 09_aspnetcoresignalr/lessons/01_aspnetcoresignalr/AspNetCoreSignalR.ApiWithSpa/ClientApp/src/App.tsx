import React from "react";
import Chat from "./chat/Chat";
import "./App.css";
import { Tabs } from "antd";

const { TabPane } = Tabs;
const App: React.FC = () => {
  return (
    <div className="App">
      <Tabs defaultActiveKey="1">
        <TabPane tab="Chat" key="1">
          <Chat />
        </TabPane>
        <TabPane tab="Wetter" key="2">
          Content of Tab Pane 2
        </TabPane>
      </Tabs>
    </div>
  );
};

export default App;
