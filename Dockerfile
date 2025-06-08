# Этап 1: базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Этап 2: образ для сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["server1.1/server1.1.csproj", "server1.1/"]
RUN dotnet restore "server1.1/server1.1.csproj"

# Копируем все файлы и собираем проект
COPY . .
WORKDIR /src/server1.1
RUN dotnet build "server1.1.csproj" -c Release -o /app/build

# Этап 3: публикация
FROM build AS publish
RUN dotnet publish "server1.1.csproj" -c Release -o /app/publish

# Этап 4: финальный образ для запуска
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "server1.1.dll"]
