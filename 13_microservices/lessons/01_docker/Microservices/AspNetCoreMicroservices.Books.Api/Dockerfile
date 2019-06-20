FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["AspNetCoreMicroservices.Books.Api/AspNetCoreMicroservices.Books.Api.csproj", "AspNetCoreMicroservices.Books.Api/"]
RUN dotnet restore "AspNetCoreMicroservices.Books.Api/AspNetCoreMicroservices.Books.Api.csproj"
COPY . .
WORKDIR "/src/AspNetCoreMicroservices.Books.Api"
RUN dotnet build "AspNetCoreMicroservices.Books.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AspNetCoreMicroservices.Books.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCoreMicroservices.Books.Api.dll"]
