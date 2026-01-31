# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os projetos
COPY ["SistemaNotificacao.Worker/SistemaNotificacao.Worker.csproj", "SistemaNotificacao.Worker/"]
COPY ["SistemaBase.Shared/SistemaBase.Shared.csproj", "SistemaBase.Shared/"]

RUN dotnet restore "SistemaNotificacao.Worker/SistemaNotificacao.Worker.csproj"

# Copia o código e builda
COPY . .
WORKDIR "/src/SistemaNotificacao.Worker"
RUN dotnet build "SistemaNotificacao.Worker.csproj" -c Release -o /app/build

# Publicação
FROM build AS publish
RUN dotnet publish "SistemaNotificacao.Worker.csproj" -c Release -o /app/publish

# Imagem Final (Runtime simples para Workers)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaNotificacao.Worker.dll"]