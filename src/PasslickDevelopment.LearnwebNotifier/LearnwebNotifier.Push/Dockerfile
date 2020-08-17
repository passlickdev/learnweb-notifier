FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["LearnwebNotifier.Push/LearnwebNotifier.Push.csproj", "LearnwebNotifier.Push/"]
COPY ["LearnwebNotifier.Library/LearnwebNotifier.Library.csproj", "LearnwebNotifier.Library/"]
RUN dotnet restore "LearnwebNotifier.Push/LearnwebNotifier.Push.csproj"
COPY . .
WORKDIR "/src/LearnwebNotifier.Push"
RUN dotnet build "LearnwebNotifier.Push.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LearnwebNotifier.Push.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "lwnotif-push.dll"]