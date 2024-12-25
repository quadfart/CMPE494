FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PlanAppAPI/PlanAppAPI/PlanAppAPI.csproj", "PlanAppAPI/"]
COPY ["PlanAppAPI/Domain/Domain.csproj", "Domain/"]
COPY ["PlanAppAPI/Persistence/Persistence.csproj", "Persistence/"]
RUN dotnet restore "PlanAppAPI/PlanAppAPI.csproj"
COPY . .
WORKDIR "/src/PlanAppAPI"
RUN dotnet build "PlanAppAPI/PlanAppAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# dotnet-ef aracı kurulumunu yap
RUN dotnet tool install --global dotnet-ef --version 7.0.20

# PATH değişkenini güncelle
ENV PATH="$PATH:/root/.dotnet/tools"

#RUN dotnet ef database update -p Persistence/Persistence.csproj -s PlanAppAPI/PlanAppAPI.csproj

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PlanAppAPI/PlanAppAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY PlanAppAPI/PlanAppAPI/Seeds/PlantData.xlsx .
ENTRYPOINT ["dotnet", "PlanAppAPI.dll"]
