CREATE TABLE [dbo].[OpenIddictScopes] (
    [Id]               NVARCHAR (450) NOT NULL,
    [ConcurrencyToken] NVARCHAR (50)  NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [Descriptions]     NVARCHAR (MAX) NULL,
    [DisplayName]      NVARCHAR (MAX) NULL,
    [DisplayNames]     NVARCHAR (MAX) NULL,
    [Name]             NVARCHAR (200) NULL,
    [Properties]       NVARCHAR (MAX) NULL,
    [Resources]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_OpenIddictScopes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_OpenIddictScopes_Name]
    ON [dbo].[OpenIddictScopes]([Name] ASC) WHERE ([Name] IS NOT NULL);

