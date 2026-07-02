FROM atendai/evolution-api:v1.8.7
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SoporteMida.Api.csproj ./
RUN dotnet restore SoporteMida.Api.csproj

COPY . ./
RUN dotnet publish SoporteMida.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "SoporteMida.Api.dll"]