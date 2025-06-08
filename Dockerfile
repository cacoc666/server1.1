# Этап 1: базовый образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Этап 2: сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ✅ Указан путь до .csproj
COPY ["server1.1/server1.1.csproj", "server1.1/"]
RUN dotnet restore "server1.1/server1.1.csproj"

# ✅ Копируем весь проект
COPY . .
WORKDIR /src/server1.1
RUN dotnet build "server1.1.csproj" -c Release -o /app/build

# Этап 3: публикация
FROM build AS publish
RUN dotnet publish "server1.1.csproj" -c Release -o /app/publish

# Этап 4: финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "server1.1.dll"]
