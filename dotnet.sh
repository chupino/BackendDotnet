#!/bin/bash

# Variables
MIGRATION_NAME="Initial"

# Verificar si la migración ya existe
if dotnet ef migrations list | grep -q "$MIGRATION_NAME"; then
    echo "La migración '$MIGRATION_NAME' ya existe."
else
    echo "Agregando la migración '$MIGRATION_NAME'."
    dotnet ef migrations add "$MIGRATION_NAME"
fi

# Aplicar migraciones a la base de datos
echo "Aplicando migraciones a la base de datos."
dotnet ef database update

# Iniciar la aplicación
echo "Iniciando la aplicación."
dotnet run --urls "http://0.0.0.0:80"

