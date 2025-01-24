# Fase 1: Base para execução no VS ou produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Fase 2: Compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia a solução para restauração de dependências
COPY ["ChaveDoSaberApi.sln", "./"]

# Copia o projeto WebAPI e restaura dependências
COPY ["src/Applications/WebAPI/WebAPI.csproj", "src/Applications/WebAPI/"]
RUN dotnet restore "src/Applications/WebAPI/WebAPI.csproj"

# Copia todo o código e compila
COPY . .
WORKDIR "/src/src/Applications/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

# Fase 3: Publicação
FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Fase 4: Execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
