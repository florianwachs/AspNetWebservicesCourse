FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src

# NodeJs ist zum Bauen des Frontends noetig
RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

COPY ["AspNetCoreMicroservices.Frontend/AspNetCoreMicroservices.Frontend.csproj", "AspNetCoreMicroservices.Frontend/"]
RUN dotnet restore "AspNetCoreMicroservices.Frontend/AspNetCoreMicroservices.Frontend.csproj"
COPY . .
WORKDIR "/src/AspNetCoreMicroservices.Frontend"
RUN dotnet build "AspNetCoreMicroservices.Frontend.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AspNetCoreMicroservices.Frontend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCoreMicroservices.Frontend.dll"]
