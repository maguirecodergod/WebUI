# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/WebUI/blazorwasm/LHA.BlazorWasm.App/LHA.BlazorWasm.App.csproj", "src/WebUI/blazorwasm/LHA.BlazorWasm.App/"]
COPY ["src/WebUI/blazorwasm/LHA.BlazorWasm.Components/LHA.BlazorWasm.Components.csproj", "src/WebUI/blazorwasm/LHA.BlazorWasm.Components/"]
COPY ["src/WebUI/blazorwasm/LHA.BlazorWasm.HttpApi.Client/LHA.BlazorWasm.HttpApi.Client.csproj", "src/WebUI/blazorwasm/LHA.BlazorWasm.HttpApi.Client/"]
COPY ["src/WebUI/blazorwasm/LHA.BlazorWasm.Modules/LHA.BlazorWasm.Modules.csproj", "src/WebUI/blazorwasm/LHA.BlazorWasm.Modules/"]
COPY ["src/WebUI/blazorwasm/LHA.BlazorWasm.Services/LHA.BlazorWasm.Services.csproj", "src/WebUI/blazorwasm/LHA.BlazorWasm.Services/"]
COPY ["src/WebUI/blazorwasm/LHA.BlazorWasm.Shared/LHA.BlazorWasm.Shared.csproj", "src/WebUI/blazorwasm/LHA.BlazorWasm.Shared/"]

RUN dotnet restore "src/WebUI/blazorwasm/LHA.BlazorWasm.App/LHA.BlazorWasm.App.csproj"

# Copy the rest of the source code
COPY . .

# Publish the app
RUN dotnet publish "src/WebUI/blazorwasm/LHA.BlazorWasm.App/LHA.BlazorWasm.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Serve Stage
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Clear default nginx static assets
RUN rm -rf ./*

# Copy published files from build stage
COPY --from=build /app/publish/wwwroot .

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
