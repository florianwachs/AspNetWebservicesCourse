#!/usr/bin/env bash
dotnet publish ../src/akkasample.App/akkasample.App.csproj --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer