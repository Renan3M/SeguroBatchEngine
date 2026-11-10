CREATE DATABASE SeguroDb;
GO

USE SeguroDb;
GO

CREATE TABLE Apolices (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    NumeroApolice VARCHAR(50) NOT NULL,
    CpfCnpjSegurado VARCHAR(20) NOT NULL,
    ValorPremio DECIMAL(18, 2) NOT NULL,
    DataInicioVigencia DATETIME2 NOT NULL,
    DataFimVigencia DATETIME2 NOT NULL,
    MetodoIngestao VARCHAR(20) NOT NULL, -- 'Regex' ou 'AI_SemanticKernel'
    DataProcessamento DATETIME2 DEFAULT SYSUTCDATETIME()
);
GO