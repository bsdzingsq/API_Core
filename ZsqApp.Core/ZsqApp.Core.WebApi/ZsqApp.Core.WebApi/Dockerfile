FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY ["ZsqApp.Core.WebApi/ZsqApp.Core.WebApi/ZsqApp.Core.WebApi.csproj", "ZsqApp.Core.WebApi/ZsqApp.Core.WebApi/"]
COPY ["ZsqApp.Core.Entity/ZsqApp.Core.Entity.csproj", "ZsqApp.Core.Entity/"]
COPY ["ZsqApp.Core.Models/ZsqApp.Core.Models.csproj", "ZsqApp.Core.Models/"]
COPY ["ZsqApp.Core.Infrastructure/ZsqApp.Core.Infrastructure.csproj", "ZsqApp.Core.Infrastructure/"]
COPY ["ZsqApp.Core.Services/ZsqApp.Core.Services.csproj", "ZsqApp.Core.Services/"]
COPY ["ZsqApp.Core.Interfaces/ZsqApp.Core.Interfaces.csproj", "ZsqApp.Core.Interfaces/"]
COPY ["ZsqApp.Core.ViewModel/ZsqApp.Core.ViewModel.csproj", "ZsqApp.Core.ViewModel/"]
RUN dotnet restore "ZsqApp.Core.WebApi/ZsqApp.Core.WebApi/ZsqApp.Core.WebApi.csproj"
COPY . .
WORKDIR "/src/ZsqApp.Core.WebApi/ZsqApp.Core.WebApi"
RUN dotnet build "ZsqApp.Core.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ZsqApp.Core.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ZsqApp.Core.WebApi.dll"]