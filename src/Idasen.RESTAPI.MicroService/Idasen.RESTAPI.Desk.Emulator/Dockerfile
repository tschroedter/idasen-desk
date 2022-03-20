#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Idasen.RESTAPI.Desk.Emulator/Idasen.RESTAPI.Desk.Emulator.csproj", "Idasen.RESTAPI.Desk.Emulator/"]
RUN dotnet restore "Idasen.RESTAPI.Desk.Emulator/Idasen.RESTAPI.Desk.Emulator.csproj"
COPY . .
WORKDIR "/src/Idasen.RESTAPI.Desk.Emulator"
RUN dotnet build "Idasen.RESTAPI.Desk.Emulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Idasen.RESTAPI.Desk.Emulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Idasen.RESTAPI.Desk.Emulator.dll"]