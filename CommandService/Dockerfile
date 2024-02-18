##################################################
# Build Environment                              #
##################################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out

##################################################
# Runtime Environment                            #
##################################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-env
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CommandService.dll"]