FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Powerplants.Challenge.Api/Powerplants.Challenge.Api.csproj", "Powerplants.Challenge.Api/"]
COPY ["Powerplants.Challenge.Application/Powerplants.Challenge.Application.csproj", "Powerplants.Challenge.Application/"]
COPY ["Powerplants.Challenge.Domain/Powerplants.Challenge.Domain.csproj", "Powerplants.Challenge.Domain/"]
RUN dotnet restore "Powerplants.Challenge.Api/Powerplants.Challenge.Api.csproj"

COPY . .
WORKDIR "/src/Powerplants.Challenge.Api"
RUN dotnet publish "Powerplants.Challenge.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8888
ENV ASPNETCORE_URLS=http://+:8888

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Powerplants.Challenge.Api.dll"]
