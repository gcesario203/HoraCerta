FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/HoraCerta.Api/HoraCerta.Api.csproj src/HoraCerta.Api/
COPY src/HoraCerta.Aplicacao/HoraCerta.Aplicacao.csproj src/HoraCerta.Aplicacao/
COPY src/HoraCerta.Dominio/HoraCerta.Dominio.csproj src/HoraCerta.Dominio/
COPY src/HoraCerta.Infaestrutura/HoraCerta.Infaestrutura.csproj src/HoraCerta.Infaestrutura/
COPY src/HoraCerta.Transversal/HoraCerta.Transversal.csproj src/HoraCerta.Transversal/

RUN dotnet restore src/HoraCerta.Api/HoraCerta.Api.csproj

COPY src/ ./src/
WORKDIR /src/HoraCerta.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HoraCerta.Api.dll"]
