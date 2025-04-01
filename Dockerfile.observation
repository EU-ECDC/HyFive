# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update
RUN apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs

FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HyFive.Observasjon/Directory.Build.props", "HyFive.Observasjon/"]
COPY ["HyFive.Observasjon/HyFive.Observasjon.csproj", "HyFive.Observasjon/"]
COPY ["HyFive.Api.Common/HyFive.Api.Common.csproj", "HyFive.Api.Common/"]
COPY ["HyFive.Modeller/HyFive.Modeller.csproj", "HyFive.Modeller/"]
COPY ["HyFive.Tjenester.Autentisering/HyFive.Tjenester.Autentisering.csproj", "HyFive.Tjenester.Autentisering/"]
COPY ["HyFive.Dataaksess/HyFive.Dataaksess.csproj", "HyFive.Dataaksess/"]
COPY ["HyFive.Domene/HyFive.Domene.csproj", "HyFive.Domene/"]
COPY ["HyFive.Tjenester.Rapporter/HyFive.Tjenester.Rapporter.csproj", "HyFive.Tjenester.Rapporter/"]
COPY ["HyFive.Tjenester/HyFive.Tjenester.csproj", "HyFive.Tjenester/"]
RUN dotnet restore "./HyFive.Observasjon/HyFive.Observasjon.csproj"
COPY . .
WORKDIR "/src/HyFive.Observasjon"
RUN dotnet build "./HyFive.Observasjon.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HyFive.Observasjon.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HyFive.Observasjon.dll"]