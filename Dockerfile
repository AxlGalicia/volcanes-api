FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 443
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["volcanes-api/volcanes-api.csproj", "volcanes-api/"]
RUN dotnet restore "volcanes-api/volcanes-api.csproj"
COPY . .
WORKDIR "/src/volcanes-api"
RUN dotnet build "volcanes-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "volcanes-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "volcanes-api.dll"]
