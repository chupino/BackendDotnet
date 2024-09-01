FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR app
RUN dotnet new webapi -n Backend
WORKDIR Backend
RUN mkdir htmls
COPY doc*.html ./htmls
COPY Program.cs .
COPY appsettings*.json .
COPY ApplicationDbContext.cs .
COPY Backend.csproj .
COPY ApiController.cs ./Controllers/
COPY DataSeeder.cs .
RUN dotnet build
RUN dotnet tool install --global dotnet-ef
ENTRYPOINT dotnet run --urls "http://0.0.0.0:80"