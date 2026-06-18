FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Okane/Okane.csproj", "Okane/"]
RUN dotnet restore "Okane/Okane.csproj"

COPY . .
WORKDIR "./Okane"
RUN dotnet build "Okane.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Okane.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Okane.dll"]