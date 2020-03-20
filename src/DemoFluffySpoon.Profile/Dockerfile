FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.2 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.200 AS publish
WORKDIR /sln
COPY NuGet.Config .
COPY packages/ ./packages
COPY ["src/DemoFluffySpoon.Profile/DemoFluffySpoon.Profile.csproj", "src/DemoFluffySpoon.Profile/"]
RUN dotnet restore "src/DemoFluffySpoon.Profile/DemoFluffySpoon.Profile.csproj"
COPY . .
WORKDIR "src/DemoFluffySpoon.Profile"
ARG VERSION=1
RUN dotnet publish "DemoFluffySpoon.Profile.csproj" /p:Version=$VERSION -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoFluffySpoon.Profile.dll"]