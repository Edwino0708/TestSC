# Utiliza la imagen oficial de .NET 5 SDK como la etapa de build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copia el archivo de solución y los archivos de proyecto
COPY *.sln ./
COPY TestSimetricaConsulting/*.csproj ./TestSimetricaConsulting/
COPY UnitTestSimetricaConsulting/*.csproj ./UnitTestSimetricaConsulting/
COPY Domain/*.csproj ./Domain/
COPY DataAcessRepository/*.csproj ./DataAcessRepository/
COPY DataAccessPackage/*.csproj ./DataAccessPackage/

# Restaura las dependencias
RUN dotnet restore

# Copia el resto de los archivos del proyecto y compila la aplicación
COPY . .
RUN dotnet publish -c Release -o out

# Utiliza la imagen de runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .

# Define la entrada para el contenedor
ENTRYPOINT ["dotnet", "WebService.dll"]
