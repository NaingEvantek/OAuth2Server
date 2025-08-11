CREATE TABLE [dbo].[OpenIddictApplications] (
    [Id]                     NVARCHAR (450) NOT NULL,
    [ClientId]               NVARCHAR (100) NULL,
    [ClientSecret]           NVARCHAR (MAX) NULL,
    [ConcurrencyToken]       NVARCHAR (50)  NULL,
    [ConsentType]            NVARCHAR (50)  NULL,
    [DisplayName]            NVARCHAR (MAX) NULL,
    [DisplayNames]           NVARCHAR (MAX) NULL,
    [Permissions]            NVARCHAR (MAX) NULL,
    [PostLogoutRedirectUris] NVARCHAR (MAX) NULL,
    [Properties]             NVARCHAR (MAX) NULL,
    [RedirectUris]           NVARCHAR (MAX) NULL,
    [Requirements]           NVARCHAR (MAX) NULL,
    [Type]                   NVARCHAR (50)  NULL,
    CONSTRAINT [PK_OpenIddictApplications] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_OpenIddictApplications_ClientId]
    ON [dbo].[OpenIddictApplications]([ClientId] ASC) WHERE ([ClientId] IS NOT NULL);

