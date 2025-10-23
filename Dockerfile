# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/Travel.Api ./Travel.Api
RUN dotnet restore ./Travel.Api/Travel.Api.csproj
RUN dotnet publish ./Travel.Api/Travel.Api.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./
# Expose 8080 and configure Kestrel to listen on 0.0.0.0:8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Travel.Api.dll"]
