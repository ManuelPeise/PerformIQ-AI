FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["sources/Web.App/Web.App/Web.App.csproj", "sources/Web.App/Web.App/"]
COPY ["sources/Web.App/Web.App.Client/Web.App.Client.csproj", "sources/Web.App/Web.App.Client/"]
COPY ["sources/Data.Accessor/Data.Accessor.csproj", "sources/Data.Accessor/"]
COPY ["sources/Data.Database/Data.Database.csproj", "sources/Data.Database/"]
COPY ["sources/Logic.Authentication/Logic.Authentication.csproj", "sources/Logic.Authentication/"]
COPY ["sources/Logic.Modules/Logic.Modules.csproj", "sources/Logic.Modules/"]
COPY ["sources/Logic.Shared/Logic.Shared.csproj", "sources/Logic.Shared/"]
COPY ["sources/Shared.Models/Shared.Models.csproj", "sources/Shared.Models/"]
RUN dotnet restore "sources/Web.App/Web.App/Web.App.csproj"

COPY . .
WORKDIR /src/sources/Web.App/Web.App
RUN dotnet publish "Web.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.App.dll"]
