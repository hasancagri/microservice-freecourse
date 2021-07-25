#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Gateways/FreeCourse.Gateway/FreeCourse.Gateway.csproj", "Gateways/FreeCourse.Gateway/"]
RUN dotnet restore "Gateways/FreeCourse.Gateway/FreeCourse.Gateway.csproj"
COPY . .
WORKDIR "/src/Gateways/FreeCourse.Gateway"
RUN dotnet build "FreeCourse.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FreeCourse.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FreeCourse.Gateway.dll"]