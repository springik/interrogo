FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Dependencies
COPY ["interrogo.sln", "./"]
COPY ["Web/*.csproj", "Web/"]
COPY ["Core/*.csproj", "Core/"]
COPY ["Infrastructure/*.csproj", "Infrastructure/"]
RUN dotnet restore "Web/Web.csproj"

# Copy source & build
COPY . .
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# RUn
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]