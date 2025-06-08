# Этап 1: базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Этап 2: сборка проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем проектный файл и восстанавливаем зависимости
COPY ["server1.1.csproj", "./"]
RUN dotnet restore "server1.1.csproj"

# Копируем все остальные файлы и собираем проект
COPY . .
RUN dotnet build "server1.1.csproj" -c Release -o /app/build

# Этап 3: публикация
FROM build AS publish
RUN dotnet publish "server1.1.csproj" -c Release -o /app/publish

# Этап 4: финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "server1.1.dll"]
