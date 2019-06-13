import React, { PureComponent } from "react";
import * as signalR from "@aspnet/signalr";
import { Layout, Input, Button, Card } from "antd";
const { Header, Footer, Sider, Content } = Layout;
class Weather extends PureComponent<{}, IWeatherState> {
  private connection: signalR.HubConnection = new signalR.HubConnectionBuilder().withUrl("/weatherHub").build();

  state: IWeatherState = {
    isConnected: false,
    forcast: {
      summary: "unknown",
      temperatureC: "unknown",
      dateFormatted: "unknown"
    }
  };
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
        <Content>
          <Card>
            <p>Datum: {forcast.dateFormatted}</p>
            <p>Temperatur in C: {forcast.temperatureC}</p>
            <p>Zusammenfassung: {forcast.summary}</p>
          </Card>
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

    this.connection.on("weatherUpdated", (forcast: IWeatherForcast) => {
      console.log(forcast);
      this.setState({ forcast: forcast });
    });
  }
}

export default Weather;

interface IWeatherState {
  isConnected: boolean;
  forcast: IWeatherForcast;
}

interface IWeatherForcast {
  dateFormatted: string;
  temperatureC: string;
  summary: string;
}
