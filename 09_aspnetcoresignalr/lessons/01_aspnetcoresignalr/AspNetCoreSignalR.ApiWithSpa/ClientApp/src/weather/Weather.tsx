import React, { PureComponent } from "react";
import * as signalR from "@aspnet/signalr";
import { Layout, Input, Button } from "antd";
const { Header, Footer, Sider, Content } = Layout;
class Weather extends PureComponent<{}, IWeatherState> {
  private connection: signalR.HubConnection = new signalR.HubConnectionBuilder().withUrl("/weatherHub").build();

  state: IWeatherState = { isConnected: false };
  componentDidMount() {
    this.connectToSignalR();
  }

  componentWillUnmount() {
    if (this.connection) {
      this.connection.stop();
    }
  }
  render() {
    const { isConnected, forcast } = this.state;

    return (
      <Layout>
        <Header>My-Chat</Header>
        <Content />
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

    this.connection.on("weatherUpdated", (forcast: IWeatherForcast) => {
      console.log(forcast);
      this.setState({ forcast: forcast });
    });
  }
}

export default Weather;

interface IWeatherState {
  isConnected: boolean;
  forcast?: IWeatherForcast;
}

interface IWeatherForcast {
  dateFormatted: string;
  temperatureC: number;
  summary: string;
  temperatureF: number;
}
