# Use the official .NET Core SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /source

# Copy the project file and restore dependencies
COPY myservice/myservice.csproj ./
RUN dotnet restore

# Copy the remaining source code and build the application
COPY myservice/. ./
RUN dotnet publish -c Release -o /app --no-restore

# COPY ./ .
# RUN dotnet publish -c Release -o /app

# Use the official ASP.NET Core runtime image as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ARG GIT_COMMIT_SHA

ENV NEW_RELIC_METADATA_REPOSITORY_URL=https://github.com/hjnz2023/MultiServices
ENV NEW_RELIC_METADATA_COMMIT=$GIT_COMMIT_SHA

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

COPY newrelic.config /usr/local/newrelic-dotnet-agent/newrelic.config
ENV NEW_RELIC_APP_NAME=myservice

WORKDIR /app
COPY --from=build-env /app ./

# Start the application
ENTRYPOINT ["/usr/local/newrelic-dotnet-agent/run.sh", "dotnet", "myservice.dll"]