FROM mcr.microsoft.com/dotnet/sdk:9.0@sha256:3fcf6f1e809c0553f9feb222369f58749af314af6f063f389cbd2f913b4ad556 AS build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out -c Release --os linux -p:DebugSymbols=false -p:PublishSingleFile=true --self-contained

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["./SEGBot"]