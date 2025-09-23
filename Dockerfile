# BASE IMAGE (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Railway injects PORT; make Kestrel bind to it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# optional: default port you use locally
EXPOSE 8080

# BUILD IMAGE (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy only csproj first for better caching , make sure you edit it 
COPY Deploying_Test.csproj ./
RUN dotnet restore

# now copy the rest
COPY . ./

# publish
RUN dotnet publish -c Release -o /out /p:UseAppHost=false

# FINAL IMAGE
FROM base AS final
WORKDIR /app
COPY --from=build /out .

# IMPORTANT: point to your dll name , make sure you edit it 
ENTRYPOINT ["dotnet", "Deploying_Test.dll"]
