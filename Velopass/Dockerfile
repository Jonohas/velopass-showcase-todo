FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Ensure Kestrel listens on port 8080 inside the container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VelopassShowcaseTodo/VelopassShowcaseTodo.csproj", "VelopassShowcaseTodo/"]
RUN dotnet restore "VelopassShowcaseTodo/VelopassShowcaseTodo.csproj"
COPY . .
WORKDIR "/src/VelopassShowcaseTodo"
RUN dotnet build "./VelopassShowcaseTodo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VelopassShowcaseTodo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VelopassShowcaseTodo.dll"]
