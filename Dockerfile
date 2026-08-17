# --- 1. Сборка приложения ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем все файлы проекта
COPY . .

# Восстанавливаем зависимости (dotnet сам найдет .csproj или .sln в любой папке!)
RUN dotnet restore

# Собираем Release
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# --- 2. Финальный образ ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Устанавливаем docker-cli для песочницы
RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://download.docker.com/linux/static/stable/x86_64/docker-26.1.4.tgz | tar -xz -C /tmp && \
    mv /tmp/docker/docker /usr/local/bin/ && \
    rm -rf /tmp/docker

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# ВНИМАНИЕ: Проверь, чтобы название dll совпадало с именем твоего проекта
ENTRYPOINT ["dotnet", "CSharp_teacher.dll"]