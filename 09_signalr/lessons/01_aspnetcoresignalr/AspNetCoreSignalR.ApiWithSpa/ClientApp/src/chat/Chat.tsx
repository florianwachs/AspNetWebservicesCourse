import React, { PureComponent, useState } from "react";
import * as signalR from "@aspnet/signalr";
import { Layout, Input, Button } from "antd";
const { Header, Footer, Sider, Content } = Layout;
class Chat extends PureComponent<{}, IChatState> {
  private connection: signalR.HubConnection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

  state: IChatState = { isConnected: false, text: "", userName: "", messages: [] };
  componentDidMount() {
    this.connectToSignalR();
  }

  componentWillUnmount() {
    if (this.connection) {
      this.connection.stop();
    }
  }
  render() {
    const { isConnected, text, messages } = this.state;

    return (
      <Layout>
        <Header>My-Chat</Header>
        <Content>
          <div>
            <Input onChange={e => this.setState({ text: e.target.value })} />
            <Button onClick={this.send} disabled={!text}>
              Senden
            </Button>
          </div>
          <div>
            <ul>
              {messages.map(message => (
                <li>{message}</li>
              ))}
            </ul>
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
      .catch(err => document.write(err));

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
