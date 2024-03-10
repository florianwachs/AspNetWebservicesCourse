// package: 
// file: WeatherSensor.proto

var WeatherSensor_pb = require("./WeatherSensor_pb");
var google_protobuf_empty_pb = require("google-protobuf/google/protobuf/empty_pb");
var grpc = require("@improbable-eng/grpc-web").grpc;

var SensorReadingService = (function () {
  function SensorReadingService() {}
  SensorReadingService.serviceName = "SensorReadingService";
  return SensorReadingService;
}());

SensorReadingService.AddReading = {
  methodName: "AddReading",
  service: SensorReadingService,
  requestStream: false,
  responseStream: false,
  requestType: WeatherSensor_pb.SensorReadingPackage,
  responseType: WeatherSensor_pb.SensorResponseMessage
};

SensorReadingService.GetUpdates = {
  methodName: "GetUpdates",
  service: SensorReadingService,
  requestStream: false,
  responseStream: true,
  requestType: google_protobuf_empty_pb.Empty,
  responseType: WeatherSensor_pb.SensorReadingMessage
};

exports.SensorReadingService = SensorReadingService;

function SensorReadingServiceClient(serviceHost, options) {
  this.serviceHost = serviceHost;
  this.options = options || {};
}

SensorReadingServiceClient.prototype.addReading = function addReading(requestMessage, metadata, callback) {
  if (arguments.length === 2) {
    callback = arguments[1];
  }
  var client = grpc.unary(SensorReadingService.AddReading, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onEnd: function (response) {
      if (callback) {
        if (response.status !== grpc.Code.OK) {
          var err = new Error(response.statusMessage);
          err.code = response.status;
          err.metadata = response.trailers;
          callback(err, null);
        } else {
          callback(null, response.message);
        }
      }
    }
  });
  return {
    cancel: function () {
      callback = null;
      client.close();
    }
  };
};

SensorReadingServiceClient.prototype.getUpdates = function getUpdates(requestMessage, metadata) {
  var listeners = {
    data: [],
    end: [],
    status: []
  };
  var client = grpc.invoke(SensorReadingService.GetUpdates, {
    request: requestMessage,
    host: this.serviceHost,
    metadata: metadata,
    transport: this.options.transport,
    debug: this.options.debug,
    onMessage: function (responseMessage) {
      listeners.data.forEach(function (handler) {
        handler(responseMessage);
      });
    },
    onEnd: function (status, statusMessage, trailers) {
      listeners.status.forEach(function (handler) {
        handler({ code: status, details: statusMessage, metadata: trailers });
      });
      listeners.end.forEach(function (handler) {
        handler({ code: status, details: statusMessage, metadata: trailers });
      });
      listeners = null;
    }
  });
  return {
    on: function (type, handler) {
      listeners[type].push(handler);
      return this;
    },
    cancel: function () {
      listeners = null;
      client.close();
    }
  };
};

exports.SensorReadingServiceClient = SensorReadingServiceClient;

