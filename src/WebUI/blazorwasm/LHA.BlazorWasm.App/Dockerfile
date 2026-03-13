# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy everything and restore
COPY . .
RUN dotnet restore "src/WebUI/blazorwasm/LHA.BlazorWasm.App/LHA.BlazorWasm.App.csproj"

# Publish the app
RUN dotnet publish "src/WebUI/blazorwasm/LHA.BlazorWasm.App/LHA.BlazorWasm.App.csproj" -c Release -o /app/publish

# Runtime Stage (Nginx)
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy published files from build stage
COPY --from=build /app/publish/wwwroot .

# Copy custom nginx configuration
COPY src/WebUI/blazorwasm/LHA.BlazorWasm.App/nginx.conf /etc/nginx/nginx.conf

# Render expects the app to listen on a port, usually 80 or 10000
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
