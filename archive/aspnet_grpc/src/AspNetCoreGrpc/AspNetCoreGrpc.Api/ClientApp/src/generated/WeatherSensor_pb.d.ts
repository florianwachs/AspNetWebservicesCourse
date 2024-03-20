// package: 
// file: WeatherSensor.proto

import * as jspb from "google-protobuf";
import * as google_protobuf_timestamp_pb from "google-protobuf/google/protobuf/timestamp_pb";
import * as google_protobuf_empty_pb from "google-protobuf/google/protobuf/empty_pb";
import * as enums_pb from "./enums_pb";

export class SensorReadingPackage extends jspb.Message {
  clearReadingsList(): void;
  getReadingsList(): Array<SensorReadingMessage>;
  setReadingsList(value: Array<SensorReadingMessage>): void;
  addReadings(value?: SensorReadingMessage, index?: number): SensorReadingMessage;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SensorReadingPackage.AsObject;
  static toObject(includeInstance: boolean, msg: SensorReadingPackage): SensorReadingPackage.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: SensorReadingPackage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SensorReadingPackage;
  static deserializeBinaryFromReader(message: SensorReadingPackage, reader: jspb.BinaryReader): SensorReadingPackage;
}

export namespace SensorReadingPackage {
  export type AsObject = {
    readingsList: Array<SensorReadingMessage.AsObject>,
  }
}

export class SensorReadingMessage extends jspb.Message {
  getSensorid(): number;
  setSensorid(value: number): void;

  getTemperatureinc(): number;
  setTemperatureinc(value: number): void;

  getHumidity(): number;
  setHumidity(value: number): void;

  getAtmosphericpressure(): number;
  setAtmosphericpressure(value: number): void;

  getSensorstatus(): enums_pb.SensorStatusMap[keyof enums_pb.SensorStatusMap];
  setSensorstatus(value: enums_pb.SensorStatusMap[keyof enums_pb.SensorStatusMap]): void;

  hasTimestamp(): boolean;
  clearTimestamp(): void;
  getTimestamp(): google_protobuf_timestamp_pb.Timestamp | undefined;
  setTimestamp(value?: google_protobuf_timestamp_pb.Timestamp): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SensorReadingMessage.AsObject;
  static toObject(includeInstance: boolean, msg: SensorReadingMessage): SensorReadingMessage.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: SensorReadingMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SensorReadingMessage;
  static deserializeBinaryFromReader(message: SensorReadingMessage, reader: jspb.BinaryReader): SensorReadingMessage;
}

export namespace SensorReadingMessage {
  export type AsObject = {
    sensorid: number,
    temperatureinc: number,
    humidity: number,
    atmosphericpressure: number,
    sensorstatus: enums_pb.SensorStatusMap[keyof enums_pb.SensorStatusMap],
    timestamp?: google_protobuf_timestamp_pb.Timestamp.AsObject,
  }
}

export class SensorResponseMessage extends jspb.Message {
  getSuccess(): boolean;
  setSuccess(value: boolean): void;

  getMessage(): string;
  setMessage(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): SensorResponseMessage.AsObject;
  static toObject(includeInstance: boolean, msg: SensorResponseMessage): SensorResponseMessage.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: SensorResponseMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): SensorResponseMessage;
  static deserializeBinaryFromReader(message: SensorResponseMessage, reader: jspb.BinaryReader): SensorResponseMessage;
}

export namespace SensorResponseMessage {
  export type AsObject = {
    success: boolean,
    message: string,
  }
}

