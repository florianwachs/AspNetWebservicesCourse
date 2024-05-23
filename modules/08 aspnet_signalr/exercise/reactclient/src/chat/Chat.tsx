import React, { PureComponent, useState } from "react";
import * as signalR from "@aspnet/signalr";
import { Layout, Input, Button, List } from "antd";
const { Header, Footer, Sider, Content } = Layout;
class Chat extends PureComponent<{}, IChatState> {
  private connection: signalR.HubConnection = new signalR.HubConnectionBuilder().withUrl("http://localhost:5272/chatHub").build();

  state: IChatState = {
    isConnected: false,
    text: "",
    userName: "",
    messages: [],
  };
  componentDidMount() {
    this.connectToSignalR();
  }

  componentWillUnmount() {
    if (this.connection) {
      this.connection.stop();
    }
  }

  defaultStyle = { margin: "0.5rem" };

  render() {
    const { isConnected, text, messages } = this.state;

    return (
      <Layout>
        <Header className="header">My-Chat</Header>
        <Content>
          <div style={this.defaultStyle}>
            <Input style={this.defaultStyle} onChange={(e) => this.setState({ text: e.target.value })} />
            <Button style={this.defaultStyle} onClick={this.send} disabled={!text}>
              Senden
            </Button>
          </div>
          <div style={this.defaultStyle}>
            <List
              className="comment-list"
              header={`Nachrichten`}
              itemLayout="horizontal"
              dataSource={messages}
              renderItem={(message) => (
                <li>
                  {message}
                </li>
              )}
            />
          </div>
        </Content>
        <Footer>{isConnected ? "Mit SignalR verbunden" : "Noop, keine Verbindung"}</Footer>
      </Layout>
    );
  }

  private connectToSignalR() {
    this.connection
      .start()
      .then(() => {
        this.setState({ isConnected: true });
      })
      .catch((err) => document.write(err));

    this.connection.on("receiveMessage", (username: string, message: string) => {
      const oldMessages = this.state.messages;
      const newMessages = [message, ...oldMessages];
      this.setState({ messages: newMessages });
    });
  }

  send = () => {
    this.connection.send("sendMessage", this.state.userName, this.state.text);
    this.setState({ text: "" });
  };
}

export default Chat;

interface IChatState {
  isConnected: boolean;
  text: string;
  userName: string;
  messages: string[];
}
