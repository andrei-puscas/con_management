-- Rulează acest script pe baza de date ConManagementDb dacă coloanele Buget și Moneda lipsesc din Proiecte.
-- Utilizare: în SQL Server Management Studio sau sqlcmd, conectează-te la .\SQLEXPRESS, baza ConManagementDb.

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'Proiecte') AND name = N'Buget')
BEGIN
    ALTER TABLE [Proiecte] ADD [Buget] decimal(18,2) NULL;
    PRINT 'Coloana Buget a fost adăugată.';
END
ELSE
    PRINT 'Coloana Buget există deja.';

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'Proiecte') AND name = N'Moneda')
BEGIN
    ALTER TABLE [Proiecte] ADD [Moneda] nvarchar(10) NULL;
    PRINT 'Coloana Moneda a fost adăugată.';
END
ELSE
    PRINT 'Coloana Moneda există deja.';
