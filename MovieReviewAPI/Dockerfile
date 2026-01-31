# ---------- BASE RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MovieReviewAPI/MovieReviewAPI.csproj", "MovieReviewAPI/"]
COPY ["MovieReview.Application/MovieReview.Application.csproj", "MovieReview.Application/"]
COPY ["MovieReview.Core/MovieReview.Core.csproj", "MovieReview.Core/"]
COPY ["MovieReview.Domain/MovieReview.Domain.csproj", "MovieReview.Domain/"]
COPY ["MovieReview.Infrastructure/MovieReview.Infrastructure.csproj", "MovieReview.Infrastructure/"]

RUN dotnet restore "MovieReviewAPI/MovieReviewAPI.csproj"

COPY . .
WORKDIR "/src/MovieReviewAPI"
RUN dotnet build "MovieReviewAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ---------- PUBLISH ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MovieReviewAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ---------- FINAL ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://*:8080
ENV PORT=8080

ENTRYPOINT ["dotnet", "MovieReviewAPI.dll"]