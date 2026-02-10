# Estagio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os projetos para restaurar depend?ncias
COPY ["SistemaPedidos.API/SistemaPedidos.API.csproj", "SistemaPedidos.API/"]
COPY ["SistemaBase.Shared/SistemaBase.Shared.csproj", "SistemaBase.Shared/"]

RUN dotnet restore "SistemaPedidos.API/SistemaPedidos.API.csproj"

# Copia o c?digo todo e builda
COPY . .
WORKDIR "/src/SistemaPedidos.API"
RUN dotnet build "SistemaPedidos.API.csproj" -c Release -o /app/build

# Publica
FROM build AS publish
RUN dotnet publish "SistemaPedidos.API.csproj" -c Release -o /app/publish

# Imagem Final (ASP.NET para APIs)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SistemaPedidos.API.dll"]