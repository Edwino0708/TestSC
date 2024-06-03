FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copia los archivos del proyecto y restaura las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto de los archivos y compila la aplicación
COPY . .
RUN dotnet publish -c Release -o out

# Crea la imagen final
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TestSimetricaConsulting.dll"]