#!/bin/bash

# Eliminar la base de datos
echo "Eliminando la base de datos..."
dotnet ef database drop --force

# Eliminar archivos de migración
echo "Eliminando archivos de migración..."
rm -rf Migrations/

# Crear nueva migración
echo "Creando nueva migración..."
dotnet ef migrations add Initial

# Aplicar migración a la base de datos
echo "Aplicando migración a la base de datos..."
dotnet ef database update

# Iniciar la aplicación
echo "Iniciando la aplicación..."
dotnet run --urls "http://0.0.0.0:80"

