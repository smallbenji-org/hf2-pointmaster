FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pointmaster/Pointmaster.csproj", "./"]
RUN dotnet restore './Pointmaster.csproj'
COPY ./Pointmaster ./
WORKDIR /src
RUN dotnet build "./Pointmaster.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM node:20 AS node-build
WORKDIR /js-app
COPY ["Pointmaster/js/package.json", "Pointmaster/js/package-lock.json", "./"]
RUN npm install
COPY Pointmaster/js/ .
RUN npm run build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pointmaster.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

COPY --from=node-build /wwwroot/ /app/publish/wwwroot/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pointmaster.dll"]
