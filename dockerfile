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
COPY ["App/Consumers.Localizacao/Consumers.Localizacao.csproj", "App/Consumers.Localizacao/"]
COPY ["Consumers.Domain/Consumers.Domain.csproj", "Consumers.Domain/"]
COPY ["Consumers.Application/Consumers.Application.csproj", "Consumers.Application/"]
COPY ["Consumers.Repository/Consumers.Repository.csproj", "Consumers.Repository/"]

# Restaura as dependências
RUN dotnet restore "Consumers.sln"

# Copia o restante do código e realiza o build
COPY . .
WORKDIR "/src/App/Consumers.Localizacao"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Etapa para publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App/Consumers.Localizacao.dll"]
