FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ENV TZ="Europe/Kiev"
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OnlineMessenger/OnlineMessenger.csproj", "OnlineMessenger/"]
RUN dotnet restore "OnlineMessenger/OnlineMessenger.csproj"
COPY . .
WORKDIR "/src/OnlineMessenger"
RUN dotnet build "OnlineMessenger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OnlineMessenger.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineMessenger.dll"]