FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY src/SimpleCliApp/*.csproj ./
RUN dotnet restore

# Copy the rest of the code
COPY src/SimpleCliApp/. ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out .

# Add a small delay to ensure Oracle is ready
CMD echo "Waiting for Oracle database to be ready..." && \
    sleep 15 && \
    echo "Running application..." && \
    dotnet SimpleCliApp.dll
