# AUTHORED-IN-APP-REPO-FOR-LOCAL-SMOKE-ONLY
# 正式 Docker image 由 DevOps 團隊接手管理
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 複製 solution 與所有 csproj 先做 restore
COPY Med3Core.PACC.sln ./
COPY Med3Core.PACC/Med3Core.PACC.csproj Med3Core.PACC/
COPY Med3Core.PACC.Shims/Med3Core.PACC.Shims.csproj Med3Core.PACC.Shims/
RUN dotnet restore Med3Core.PACC/Med3Core.PACC.csproj

# 複製全部原始碼
COPY Med3Core.PACC/ Med3Core.PACC/
COPY Med3Core.PACC.Shims/ Med3Core.PACC.Shims/
RUN dotnet publish Med3Core.PACC/Med3Core.PACC.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=3s --start-period=15s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Med3Core.PACC.dll"]
