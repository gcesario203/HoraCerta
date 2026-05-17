FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/HoraCerta.Api/HoraCerta.Api.csproj HoraCerta.Api/
COPY src/HoraCerta.Aplicacao/HoraCerta.Aplicacao.csproj HoraCerta.Aplicacao/
COPY src/HoraCerta.Dominio/HoraCerta.Dominio.csproj HoraCerta.Dominio/
COPY src/HoraCerta.Infaestrutura/HoraCerta.Infaestrutura.csproj HoraCerta.Infaestrutura/
COPY src/HoraCerta.Transversal/HoraCerta.Transversal.csproj HoraCerta.Transversal/

RUN dotnet restore HoraCerta.Api/HoraCerta.Api.csproj

COPY src/HoraCerta.Api/ HoraCerta.Api/
COPY src/HoraCerta.Aplicacao/ HoraCerta.Aplicacao/
COPY src/HoraCerta.Dominio/ HoraCerta.Dominio/
COPY src/HoraCerta.Infaestrutura/ HoraCerta.Infaestrutura/
COPY src/HoraCerta.Transversal/ HoraCerta.Transversal/

RUN dotnet publish HoraCerta.Api/HoraCerta.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HoraCerta.Api.dll"]
