# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os projetos para restaurar dependências
COPY ["SistemaPedidos.API/SistemaPedidos.API.csproj", "SistemaPedidos.API/"]
COPY ["SistemaBase.Shared/SistemaBase.Shared.csproj", "SistemaBase.Shared/"]

RUN dotnet restore "SistemaPedidos.API/SistemaPedidos.API.csproj"

# Copia o código todo e builda
COPY . .
WORKDIR "/src/SistemaPedidos.API"
RUN dotnet build "SistemaPedidos.API.csproj" -c Release -o /app/build

# Publicação
FROM build AS publish
RUN dotnet publish "SistemaPedidos.API.csproj" -c Release -o /app/publish

# Imagem Final (ASP.NET para APIs)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Porta padrão do container .NET 8 é 8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "SistemaPedidos.API.dll"]