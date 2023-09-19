FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
WORKDIR /app
EXPOSE 80
COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/
COPY Web/WebMVC/FreeCourse.Web/*.csproj Web/WebMVC/FreeCourse.Web/
RUN dotnet restore Web/WebMVC/FreeCourse.Web/*.csproj
COPY . .
RUN dotnet publish Web/WebMVC/FreeCourse.Web/*.csproj -c Release -o out
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet","FreeCourse.Web.dll" ]