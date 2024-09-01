from mcr.microsoft.com/dotnet/sdk:8.0
workdir /app
run dotnet new webapi -n Backend
workdir /app/Backend

copy doc*.html ./htmls/
copy Program.cs .
copy appsettings*.json .
copy ApplicationDbContext.cs .
copy Backend.csproj .
copy ApiController.cs ./Controllers/
copy DataSeeder.cs .
copy dotnet.sh .

run sed -i 's/\r$//' dotnet.sh
run chmod 777 dotnet.sh

run dotnet restore
run dotnet build -c Release

run dotnet tool install --global dotnet-ef
env PATH="${PATH}:/root/.dotnet/tools"

entrypoint ["./dotnet.sh"]
