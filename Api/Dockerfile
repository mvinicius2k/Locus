#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

FROM build AS publish

FROM base AS final
WORKDIR /home/site/wwwroot
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

WORKDIR /app