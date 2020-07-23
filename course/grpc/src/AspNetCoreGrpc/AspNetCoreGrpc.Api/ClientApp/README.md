# Web Grpc

## Client Generierung

```powershell
./protoc --proto_path="C:\src\github\AspNetWebservicesCourse\course\grpc\src\AspNetCoreGrpc\AspNetCoreGrpc.Api\Protos" --plugin=protoc-gen-ts="C:\src\github\AspNetWebservicesCourse\course\grpc\src\AspNetCoreGrpc\AspNetCoreGrpc.Api\ClientApp\node_modules\.bin\protoc-gen-ts.cmd" --js_out="import_style=commonjs,binary:src/app/generated" --ts_out="service=grpc-web:src/app/generated" C:\src\github\AspNetWebservicesCourse\course\grpc\src\AspNetCoreGrpc\AspNetCoreGrpc.Api\Protos\WeatherSensor.proto C:\src\github\AspNetWebservicesCourse\course\grpc\src\AspNetCoreGrpc\AspNetCoreGrpc.Api\Protos\enums.proto
```