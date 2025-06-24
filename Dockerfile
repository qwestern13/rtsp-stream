# Базовый образ с .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Копируем проект и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем исходники и собираем
COPY . ./
RUN dotnet publish -c Release -o out

# Второй этап — финальный образ с .NET Runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Установка ffmpeg
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Копируем опубликованный .NET код
COPY --from=build-env /app/out .


EXPOSE 554

ENTRYPOINT ["dotnet","VideoFinder.dll", "/videos"]