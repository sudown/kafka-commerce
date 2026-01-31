# ESTÁGIO 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto (.csproj) primeiro para aproveitar o cache de camadas
COPY ["SistemaEstoque.Worker/SistemaEstoque.Worker.csproj", "SistemaEstoque.Worker/"]
COPY ["SistemaBase.Shared/SistemaBase.Shared.csproj", "SistemaBase.Shared/"]

# Restaura as dependências
RUN dotnet restore "SistemaEstoque.Worker/SistemaEstoque.Worker.csproj"

# Copia o restante do código fonte
COPY . .

# Compila o Worker de Estoque (isso automaticamente compila o Shared como dependência)
WORKDIR "/src/SistemaEstoque.Worker"
RUN dotnet build "SistemaEstoque.Worker.csproj" -c Release -o /app/build

# ESTÁGIO 2: Publish
FROM build AS publish
RUN dotnet publish "SistemaEstoque.Worker.csproj" -c Release -o /app/publish

# ESTÁGIO 3: Runtime (Imagem final leve)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaEstoque.Worker.dll"]