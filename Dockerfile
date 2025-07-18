FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["QuadMasterApp.csproj", "./"]
RUN dotnet restore "QuadMasterApp.csproj"
COPY . .
RUN dotnet build "QuadMasterApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuadMasterApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/keys
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuadMasterApp.dll"]