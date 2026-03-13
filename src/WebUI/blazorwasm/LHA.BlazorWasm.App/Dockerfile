# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj first (better Docker caching)
COPY src/WebUI/blazorwasm/LHA.BlazorWasm.App/*.csproj src/WebUI/blazorwasm/LHA.BlazorWasm.App/
COPY src/WebUI/blazorwasm/LHA.BlazorWasm.Components/*.csproj src/WebUI/blazorwasm/LHA.BlazorWasm.Components/

# Restore dependencies
RUN dotnet restore src/WebUI/blazorwasm/LHA.BlazorWasm.App/LHA.BlazorWasm.App.csproj

# Copy full source
COPY . .

# Publish Blazor WebAssembly
RUN dotnet publish src/WebUI/blazorwasm/LHA.BlazorWasm.App/LHA.BlazorWasm.App.csproj \
    -c Release \
    -o /app/publish

# =========================
# Runtime Stage (Nginx)
# =========================
FROM nginx:alpine

# Remove default nginx site
RUN rm -rf /usr/share/nginx/html/*

# Copy Blazor build output
COPY --from=build /app/publish/wwwroot/ /usr/share/nginx/html/

# Copy custom nginx config
COPY src/WebUI/blazorwasm/LHA.BlazorWasm.App/nginx.conf /etc/nginx/nginx.conf

# Expose port for Render
EXPOSE 8080

# Start nginx
CMD ["nginx", "-g", "daemon off;"]