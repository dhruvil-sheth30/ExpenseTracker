# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching
COPY ["ExpenseTracker.Client/ExpenseTracker.Client.csproj", "ExpenseTracker.Client/"]
COPY ["ExpenseTracker.Shared/ExpenseTracker.Shared.csproj", "ExpenseTracker.Shared/"]
RUN dotnet restore "ExpenseTracker.Client/ExpenseTracker.Client.csproj"

# Copy the rest of the source code
COPY ["ExpenseTracker.Client/", "ExpenseTracker.Client/"]
COPY ["ExpenseTracker.Shared/", "ExpenseTracker.Shared/"]

# Build and publish
WORKDIR "/src/ExpenseTracker.Client"
RUN dotnet build "ExpenseTracker.Client.csproj" -c Release -o /app/build
RUN dotnet publish "ExpenseTracker.Client.csproj" -c Release -o /app/publish

# Runtime stage
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY ExpenseTracker.Client/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
EXPOSE 443
