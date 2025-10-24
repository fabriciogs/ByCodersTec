CREATE TABLE Transactions (
    Id uniqueidentifier IDENTITY(1,1) PRIMARY KEY,
    [Type] INT NOT NULL,
    [DateTime] DATETIME2 NOT NULL,
    [Value] DECIMAL(18,2) NOT NULL,
    Cpf VARCHAR(11) NOT NULL,
    [Card] VARCHAR(12) NOT NULL,
    StoreOwner VARCHAR(14) NOT NULL,
    StoreName VARCHAR(19) NOT NULL,
    ImportDate DATETIME NOT NULL,
    FileImportId uniqueidentifier NOT NULL
);
CREATE INDEX IX_Transactions_StoreName ON Transactions(StoreName);