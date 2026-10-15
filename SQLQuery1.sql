IF DB_ID('InventarioDB') IS NULL
BEGIN
    CREATE DATABASE InventarioDB;
END;
GO

USE InventarioDB;
GO

IF OBJECT_ID('dbo.Productos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Precio DECIMAL(10,2) NOT NULL,
        Cantidad INT NOT NULL
    );
END;
GO