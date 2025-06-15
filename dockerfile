# Etapa base para o runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Etapa para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Consumers.sln", "."]
COPY ["App/Consumer.Localizacao/Consumer.Localizacao.csproj", "App/Consumer.Localizacao/"]
COPY ["Consumer.Domain/Consumer.Domain.csproj", "Consumer.Domain/"]
COPY ["Consumer.Application/Consumer.Application.csproj", "Consumer.Application/"]
COPY ["Consumer.Repository/Consumer.Repository.csproj", "Consumer.Repository/"]

# Restaura as dependências
RUN dotnet restore "Consumers.sln"

# Copia o restante do código e realiza o build
COPY . .
WORKDIR "/src/App/Consumer.Localizacao"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Etapa para publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.Localizacao.dll"]
