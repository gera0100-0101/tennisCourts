# ---------- Stage 1: Build React ----------
# Stage 1: build React
FROM node:20 AS frontend-build
WORKDIR /frontend

# Копируем package.json и package-lock.json
COPY tenniscourts.client/package*.json ./
RUN npm install

# Копируем весь фронт (index.html + src)
COPY tenniscourts.client/ ./

# Чистим старые сборки и кеши
RUN rm -rf build .vite-temp

# Билдим фронт
RUN npm run build

# ---------- Stage 2: Build ASP.NET Core ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем проект
COPY TennisCourts.Server/*.csproj ./
RUN dotnet restore

COPY TennisCourts.Server/ ./
RUN dotnet publish -c Release -o /app/publish

# ---------- Stage 3: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

# Копируем бэкенд
COPY --from=build /app/publish .

# Копируем фронт из Stage 1 в wwwroot
COPY --from=frontend-build /frontend/build ./wwwroot

EXPOSE 8080

ENTRYPOINT ["dotnet", "TennisCourts.Server.dll"]

