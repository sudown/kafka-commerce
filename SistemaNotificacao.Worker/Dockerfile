# ESTAGIO 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os projetos
COPY ["SistemaNotificacao.Worker/SistemaNotificacao.Worker.csproj", "SistemaNotificacao.Worker/"]
COPY ["SistemaBase.Shared/SistemaBase.Shared.csproj", "SistemaBase.Shared/"]

RUN dotnet restore "SistemaNotificacao.Worker/SistemaNotificacao.Worker.csproj"

# Copia o codigo e builda
COPY . .
WORKDIR "/src/SistemaNotificacao.Worker"
RUN dotnet build "SistemaNotificacao.Worker.csproj" -c Release -o /app/build

# Publica
FROM build AS publish
RUN dotnet publish "SistemaNotificacao.Worker.csproj" -c Release -o /app/publish

# Imagem Final (Runtime simples para Workers)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaNotificacao.Worker.dll"]