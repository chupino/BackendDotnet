#!/bin/bash

dotnet ef migrations add Initial
dotnet ef database update
dotnet run --urls "http://0.0.0.0:80"