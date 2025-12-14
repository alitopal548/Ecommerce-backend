# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Proje dosyasını kopyala ve restore
COPY *.csproj ./
RUN dotnet restore

# Tüm dosyaları kopyala ve publish
COPY . ./
RUN dotnet publish -c Release -o out

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Render PORT env değişkenini kullan
ENV ASPNETCORE_URLS=http://+:${PORT}

COPY --from=build /app/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ECommerce.dll"]
