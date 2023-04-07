FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder

WORKDIR /source
COPY src/MirrorOnce/MirrorOnce.csproj src/MirrorOnce/MirrorOnce.csproj
COPY tests/MirrorOnce.Tests/MirrorOnce.Tests.csproj tests/MirrorOnce.Tests/MirrorOnce.Tests.csproj
COPY MirrorOnce.sln MirrorOnce.sln
RUN dotnet restore

WORKDIR /source
COPY . .
RUN set -ex \
    && dotnet --version \
    && dotnet build -c Release \
    && dotnet test -c Release \
    && dotnet publish src/MirrorOnce/MirrorOnce.csproj -c Release --output /app/

FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app
COPY --from=builder /app .

ENV USBIP_MONITOR_HOSTFSPREFIX=/hostfs \
    LOG_LEVEL=Info

LABEL maintainer "Mark Lopez <m@silvenga.com>"

ENTRYPOINT ["dotnet", "MirrorOnce.dll"]
