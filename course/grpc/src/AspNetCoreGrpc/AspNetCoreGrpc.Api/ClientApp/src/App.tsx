import React, { useEffect, useState } from "react";
import "./App.css";
import { grpc } from "@improbable-eng/grpc-web";
import { SensorReadingService } from "./generated/WeatherSensor_pb_service";
import { Empty } from "google-protobuf/google/protobuf/empty_pb";
import { SensorReadingMessage } from "./generated/WeatherSensor_pb";

const allMessages: SensorReadingMessage.AsObject[] = [];

function App() {
  let [sensorReadings, setSensorReadings] = useState<SensorReadingMessage.AsObject[]>([]);
  useEffect(() => {
    let client = grpc.client(SensorReadingService.GetUpdates, { host: "https://localhost:5001" });
    client.onHeaders((headers: grpc.Metadata) => {
      console.log("sensorReading.onHeaders", headers);
    });
    client.onMessage((message: any) => {
      let reading = message.toObject() as SensorReadingMessage.AsObject;
      allMessages.unshift(reading);
      setSensorReadings(allMessages.slice());
    });
    client.onEnd((code: grpc.Code, msg: string, trailers: grpc.Metadata) => {
      console.log("sensorReading.onEnd", code, msg, trailers);
    });
    client.start();
    client.send(new Empty());
  }, []);

  return (
    <div className="App">
      <ul>
        {sensorReadings.map((reading, idx) => (
          <li key={idx}>
            {reading.timestamp?.seconds}: {reading.humidity}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;
