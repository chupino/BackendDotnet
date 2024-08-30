FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR app
RUN dotnet new webapi -n Backend
WORKDIR Backend
RUN mkdir htmls
COPY doc1.html ./htmls
COPY ApiController.cs ./Controllers/
RUN dotnet build
ENTRYPOINT dotnet run --urls "http://0.0.0.0:80"