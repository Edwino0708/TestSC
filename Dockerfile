# Utiliza la imagen oficial de .NET 8 SDK como la etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia el archivo de solución y los archivos de proyecto
COPY *.sln ./
COPY TestSimetricaConsulting/*.csproj ./TestSimetricaConsulting/
COPY UnitTestSimetricaConsulting/*.csproj ./UnitTestSimetricaConsulting/
COPY DataAccessPackage/*.csproj ./DataAccessPackage/
COPY DataAcessRepository/*.csproj ./DataAcessRepository/
COPY Domain/*.csproj ./Domain/

# Restaura las dependencias
RUN dotnet restore

# Copia el resto de los archivos del proyecto y compila la aplicación
COPY . .

# Publica la aplicación
RUN dotnet publish -c Release -o out

# Utiliza la imagen de runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Define la entrada para el contenedor
ENTRYPOINT ["dotnet", "TestSimetricaConsulting.dll"]
