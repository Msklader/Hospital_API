# API de Gestión de Citas Médicas

Esta API permite dar de alta a Medicos, Pacientes, agendar citas, validar horarios disponibles y gestionar cancelaciones utilizando **.NET 8**, **Dapper** y **SQL Server**.

## Tecnologías utilizadas
*   **Backend:** ASP.NET Core Web API
*   **ORM:** Dapper (Micro-ORM para SQL Server)
*   **Pruebas:** xUnit / MSTest con Moq
*   **Base de Datos:** SQL Server (Stored Procedures)
*   **IDE:** Visual Studio 2022

##  Configuración de la Base de Datos
Se agregó una carpeta "Tools BD" la cual contiene el diagrama E-R de la base de datos, asi como un archivo .bak y archivos con querys para restablecimiento, a acontinuación los pasos para restablecer:

1. Crea la base de datos 'tbHospital_JPR' o algun nombre similar en Sql Server.
2. En el repositoprio esta la carpeta "Tools BD" la cual contiene la información de la base de datos.
3. Para restaurar la BD, puede restaurar el backup dbHospital.bak en la carpeta Backup o bien puede ejecutar los scripts en Tools BD/Scripts, los cuales estran ordenados: 1 crear el esquema, tablas y sps, 2 insertar registros de prueba.

## Instalación
1. Clonar el repositorio.
2. Configura tu cadena de conexión en `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=TU_SERVIDOR;Database=CitasDB;User Id=user;Password=pass;TrustServerCertificate=True;"
   }
3. Con esto tendrán las herramientas para poder levantar el servicio de la APIHospital en Visual Studio 2022 

## Pendientes
1. Tuve detalles con las pruebas unitarias. Dentro de mis capas services, respositorys y DataAcces tengo inyección de dependencias lo cual me dificultó la generación los Unitests. 
    Para resolver este tema, necesito un poco mas investigación sobre Unitest con metodos que consultan a BD. 