# ESTAGIO 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos os caminhos relativos à raiz da solução
# Isso exige que o 'docker build' seja executado na raiz da pasta do projeto
COPY ["SistemaEstoque.Worker/SistemaEstoque.Worker.csproj", "SistemaEstoque.Worker/"]
COPY ["SistemaBase.Shared/SistemaBase.Shared.csproj", "SistemaBase.Shared/"]

# Restaura as dependências usando o caminho do projeto principal
RUN dotnet restore "SistemaEstoque.Worker/SistemaEstoque.Worker.csproj"

# Copia todo o código fonte (incluindo a pasta Shared e a pasta do Worker)
COPY . .

# Compila e publica em uma única etapa para otimizar a imagem
WORKDIR "/src/SistemaEstoque.Worker"
RUN dotnet publish "SistemaEstoque.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ESTÁGIO 2: Runtime
# Para Workers, a imagem 'aspnet' por conter as libs de Hosting necessárias
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Define o ponto de entrada
ENTRYPOINT ["dotnet", "SistemaEstoque.Worker.dll"]