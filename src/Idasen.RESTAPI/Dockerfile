#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Idasen.RESTAPI/Idasen.RESTAPI.csproj", "Idasen.RESTAPI/"]
COPY ["Idasen.Launcher/Idasen.Launcher.csproj", "Idasen.Launcher/"]
COPY ["Idasen.BluetoothLE.Characteristics/Idasen.BluetoothLE.Characteristics.csproj", "Idasen.BluetoothLE.Characteristics/"]
COPY ["Idasen.Aop/Idasen.Aop.csproj", "Idasen.Aop/"]
COPY ["Idasen.BluetoothLE.Core/Idasen.BluetoothLE.Core.csproj", "Idasen.BluetoothLE.Core/"]
COPY ["Idasen.BluetoothLE.Linak/Idasen.BluetoothLE.Linak.csproj", "Idasen.BluetoothLE.Linak/"]
RUN dotnet restore "Idasen.RESTAPI/Idasen.RESTAPI.csproj"
COPY . .
WORKDIR "/src/Idasen.RESTAPI"
RUN dotnet build "Idasen.RESTAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Idasen.RESTAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Idasen.RESTAPI.dll"]