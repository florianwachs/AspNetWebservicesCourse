// package: 
// file: WeatherSensor.proto

import * as WeatherSensor_pb from "./WeatherSensor_pb";
import * as google_protobuf_empty_pb from "google-protobuf/google/protobuf/empty_pb";
import {grpc} from "@improbable-eng/grpc-web";

type SensorReadingServiceAddReading = {
  readonly methodName: string;
  readonly service: typeof SensorReadingService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof WeatherSensor_pb.SensorReadingPackage;
  readonly responseType: typeof WeatherSensor_pb.SensorResponseMessage;
};

type SensorReadingServiceGetUpdates = {
  readonly methodName: string;
  readonly service: typeof SensorReadingService;
  readonly requestStream: false;
  readonly responseStream: true;
  readonly requestType: typeof google_protobuf_empty_pb.Empty;
  readonly responseType: typeof WeatherSensor_pb.SensorReadingMessage;
};

export class SensorReadingService {
  static readonly serviceName: string;
  static readonly AddReading: SensorReadingServiceAddReading;
  static readonly GetUpdates: SensorReadingServiceGetUpdates;
}

export type ServiceError = { message: string, code: number; metadata: grpc.Metadata }
export type Status = { details: string, code: number; metadata: grpc.Metadata }

interface UnaryResponse {
  cancel(): void;
}
interface ResponseStream<T> {
  cancel(): void;
  on(type: 'data', handler: (message: T) => void): ResponseStream<T>;
  on(type: 'end', handler: (status?: Status) => void): ResponseStream<T>;
  on(type: 'status', handler: (status: Status) => void): ResponseStream<T>;
}
interface RequestStream<T> {
  write(message: T): RequestStream<T>;
  end(): void;
  cancel(): void;
  on(type: 'end', handler: (status?: Status) => void): RequestStream<T>;
  on(type: 'status', handler: (status: Status) => void): RequestStream<T>;
}
interface BidirectionalStream<ReqT, ResT> {
  write(message: ReqT): BidirectionalStream<ReqT, ResT>;
  end(): void;
  cancel(): void;
  on(type: 'data', handler: (message: ResT) => void): BidirectionalStream<ReqT, ResT>;
  on(type: 'end', handler: (status?: Status) => void): BidirectionalStream<ReqT, ResT>;
  on(type: 'status', handler: (status: Status) => void): BidirectionalStream<ReqT, ResT>;
}

export class SensorReadingServiceClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  addReading(
    requestMessage: WeatherSensor_pb.SensorReadingPackage,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: WeatherSensor_pb.SensorResponseMessage|null) => void
  ): UnaryResponse;
  addReading(
    requestMessage: WeatherSensor_pb.SensorReadingPackage,
    callback: (error: ServiceError|null, responseMessage: WeatherSensor_pb.SensorResponseMessage|null) => void
  ): UnaryResponse;
  getUpdates(requestMessage: google_protobuf_empty_pb.Empty, metadata?: grpc.Metadata): ResponseStream<WeatherSensor_pb.SensorReadingMessage>;
}

