FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

COPY SWD63A2022/SWD63A2022.csproj SWD63A2022/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY Common/Common.csproj Common/
RUN dotnet restore SWD63A2022/SWD63A2022.csproj

COPY . ./
RUN dotnet publish SWD63A2022 -c Release -o out
 
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "SWD63A2022.dll"]