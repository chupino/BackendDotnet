FROM mcr.microsoft.com/dotnet/sdk:8.0

# Establece el directorio de trabajo
WORKDIR /app

# Crea un nuevo proyecto y establece el directorio de trabajo
RUN dotnet new webapi -n Backend
WORKDIR /app/Backend

# Copia los archivos del proyecto
COPY doc*.html ./htmls/
COPY Program.cs .
COPY appsettings*.json .
COPY ApplicationDbContext.cs .
COPY Backend.csproj .
COPY ApiController.cs ./Controllers/
COPY DataSeeder.cs .
COPY dotnet.sh .

# Restaura las dependencias
RUN dotnet restore

# Construye la aplicación
RUN dotnet build -c Release

# Instala dotnet-ef globalmente
RUN dotnet tool install --global dotnet-ef

# Configura el PATH para herramientas globales
ENV PATH="${PATH}:/root/.dotnet/tools"

# Aplica las migraciones antes de iniciar la aplicación
#RUN dotnet ef database update --project ./Backend.csproj

# Inicia la aplicación
ENTRYPOINT ["./dotnet.sh"]
