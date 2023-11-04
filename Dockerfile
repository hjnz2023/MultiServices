
# Use the official .NET Core SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /source

# Copy the project file and restore dependencies
COPY *.sln ./
COPY myservice/*.csproj ./myservice/
RUN dotnet restore

# Copy the remaining source code and build the application
COPY myservice/. ./myservice/
WORKDIR /source/myservice
RUN dotnet publish -c Release -o /app --no-restore

# Use the official ASP.NET Core runtime image as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=a5acb03c5ee63dc0e9c71196ae490958FFFFNRAL

WORKDIR /app
COPY --from=build-env /app ./

# Start the application
ENTRYPOINT ["dotnet", "myservice.dll"]
