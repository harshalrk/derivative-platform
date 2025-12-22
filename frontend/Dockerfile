FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["frontend/src/TraderUI/TraderUI.csproj", "TraderUI/"]
RUN dotnet restore "TraderUI/TraderUI.csproj"
COPY frontend/src/TraderUI/ TraderUI/
WORKDIR "/src/TraderUI"
RUN dotnet build "TraderUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraderUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/keys
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TraderUI.dll"]