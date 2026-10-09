-- Crea la tabla Tickets con el mismo esquema que genera EF Core (EnsureCreated)
-- y carga los 7 tickets de ejemplo. Lo usa la verificación automática del Pull Request
-- para ejecutar sql/reporte.sql sobre datos conocidos.
IF OBJECT_ID('dbo.Tickets', 'U') IS NOT NULL DROP TABLE dbo.Tickets;

CREATE TABLE dbo.Tickets (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tickets PRIMARY KEY,
    Titulo        NVARCHAR(100) NOT NULL,
    Descripcion   NVARCHAR(500) NULL,
    Prioridad     NVARCHAR(10)  NOT NULL,
    Estado        NVARCHAR(15)  NOT NULL,
    Responsable   NVARCHAR(60)  NULL,
    FechaCreacion DATETIME2     NOT NULL,
    FechaCierre   DATETIME2     NULL
);

SET IDENTITY_INSERT dbo.Tickets ON;
INSERT INTO dbo.Tickets (Id, Titulo, Prioridad, Estado, Responsable, FechaCreacion, FechaCierre) VALUES
(1, N'Error al emitir boleta',                'Critica', 'Resuelto',   'Equipo Dev', '2026-10-01 08:00', '2026-10-01 11:30'),
(2, N'Lentitud en búsqueda',                  'Alta',    'En proceso', 'Equipo Dev', '2026-10-02 09:15', NULL),
(3, N'Cambio de texto en botón',              'Baja',    'Abierto',    NULL,         '2026-10-03 10:00', NULL),
(4, N'Reporte mensual vacío',                 'Alta',    'Cerrado',    'Equipo Dev', '2026-10-03 14:00', '2026-10-04 10:00'),
(5, N'No carga foto de perfil',               'Media',   'Abierto',    NULL,         '2026-10-05 16:20', NULL),
(6, N'Caída del módulo de pagos',             'Critica', 'Cerrado',    'Equipo Dev', '2026-10-06 07:45', '2026-10-06 09:15'),
(7, N'Error intermitente al iniciar sesión',  'Critica', 'Abierto',    NULL,         '2026-10-07 18:00', NULL);
SET IDENTITY_INSERT dbo.Tickets OFF;
