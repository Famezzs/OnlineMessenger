FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ENV TZ="Europe/Kiev"
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OnlineMessanger/OnlineMessanger.csproj", "OnlineMessanger/"]
RUN dotnet restore "OnlineMessanger/OnlineMessanger.csproj"
COPY . .
WORKDIR "/src/OnlineMessanger"
RUN dotnet build "OnlineMessanger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OnlineMessanger.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineMessanger.dll"]