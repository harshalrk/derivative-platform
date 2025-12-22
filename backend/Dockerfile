FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/src/Api/Api.csproj", "Api/"]
COPY ["backend/src/Models/Models.csproj", "Models/"]
COPY ["backend/src/Pricing/Pricing.csproj", "Pricing/"]
COPY ["backend/src/Messaging/Messaging.csproj", "Messaging/"]
COPY ["backend/src/Persistence/Persistence.csproj", "Persistence/"]
RUN dotnet restore "Api/Api.csproj"
COPY backend/src/ .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]