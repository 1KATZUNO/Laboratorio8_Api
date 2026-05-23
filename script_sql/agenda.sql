/* =====================================================================
   Script de creación de la base de datos para Agenda.Api (Laboratorio 8)
   ---------------------------------------------------------------------
   Crea (si no existe):
     · Base de datos AgendaContactos
     · Tabla dbo.Contactos (Id PK identity, Nombre, NumeroTelefonico)
     · 5 contactos de ejemplo para probar el frontend Angular

   Ejecutar en SQL Server Express / LocalDB con un usuario con permisos
   para crear bases de datos. Se puede correr varias veces sin error.

   Desde la línea de comandos:
     sqlcmd -S .\SQLEXPRESS -E -C -i script_sql\agenda.sql

   O abrir el archivo en SSMS / Azure Data Studio y ejecutar.
   ===================================================================== */

IF DB_ID(N'AgendaContactos') IS NULL
BEGIN
    PRINT 'Creando base de datos AgendaContactos...';
    CREATE DATABASE [AgendaContactos];
END
ELSE
BEGIN
    PRINT 'La base de datos AgendaContactos ya existe.';
END
GO

USE [AgendaContactos];
GO

/* ----- Tabla Contactos ----------------------------------------------- */
IF OBJECT_ID(N'dbo.Contactos', N'U') IS NULL
BEGIN
    PRINT 'Creando tabla dbo.Contactos...';
    CREATE TABLE dbo.Contactos
    (
        Id               INT            IDENTITY(1,1) NOT NULL,
        Nombre           NVARCHAR(100)  NOT NULL,
        NumeroTelefonico NVARCHAR(30)   NOT NULL,
        CONSTRAINT PK_Contactos PRIMARY KEY CLUSTERED (Id ASC)
    );
END
ELSE
BEGIN
    PRINT 'La tabla dbo.Contactos ya existe.';
END
GO

/* ----- Datos de ejemplo ---------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Contactos)
BEGIN
    PRINT 'Insertando contactos de ejemplo...';
    INSERT INTO dbo.Contactos (Nombre, NumeroTelefonico) VALUES
        (N'Ana Mora',      N'8888-1111'),
        (N'Carlos Soto',   N'8888-2222'),
        (N'Maria Rojas',   N'8888-3333'),
        (N'Diego Vargas',  N'8888-4444'),
        (N'Sofia Jimenez', N'8888-5555');
END
ELSE
BEGIN
    PRINT 'La tabla ya contiene registros, no se inserta seed.';
END
GO

PRINT 'Listo. Total de contactos:';
SELECT COUNT(*) AS Total FROM dbo.Contactos;
GO
