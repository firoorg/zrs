# Zcoin REST Service

ZRS (Zcoin REST Service) is a REST service to provide an access to Zcoin network.

## Development
### Requirements

- .NET Core 3.1
- Docker Compose

### Build

```sh
dotnet build src/Zrs.sln
```

### Start required services

```sh
docker-compose up -d
```

### Start REST service

```sh
dotnet run -p src/Zrs
```
