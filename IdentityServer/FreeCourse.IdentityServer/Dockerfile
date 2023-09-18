FROM microsoft.com/dotnet/sdk:7.0-alpine as build

WORKDIR /app

EXPOSE 80

COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/

COPY IdentityServer/FreeCourse.IdentityServer/*.csproj IdentityServer/FreeCourse.IdentityServer/

RUN dotnet restore IdentityServer/FreeCourse.IdentityServer/*.csproj

COPY . .

RUN dotnet publish IdentityServer/FreeCourse.IdentityServer/*.csproj -c Release -o out

FROM microsoft.com/dotnet/aspnet:7.0-alpine as runtime

WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "FreeCourse.Services.Catalog.dll"]
