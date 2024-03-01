# Use the official .NET 6 SDK image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
VOLUME /log
COPY --from=build /app/out .
ENV RUNNING_IN_DOCKER=true
ENTRYPOINT ["dotnet", "MultiThreadLogger.dll"]