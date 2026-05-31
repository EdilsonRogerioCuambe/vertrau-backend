FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["VertrauApi/VertrauApi.csproj", "VertrauApi/"]
RUN dotnet restore "VertrauApi/VertrauApi.csproj"

COPY . .
WORKDIR "/src/VertrauApi"
RUN dotnet build "VertrauApi.csproj" -c Release -o /app/build
RUN dotnet publish "VertrauApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VertrauApi.dll"]
