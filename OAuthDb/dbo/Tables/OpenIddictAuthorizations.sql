CREATE TABLE [dbo].[OpenIddictAuthorizations] (
    [Id]               NVARCHAR (450) NOT NULL,
    [ApplicationId]    NVARCHAR (450) NULL,
    [ConcurrencyToken] NVARCHAR (50)  NULL,
    [CreationDate]     DATETIME2 (7)  NULL,
    [Properties]       NVARCHAR (MAX) NULL,
    [Scopes]           NVARCHAR (MAX) NULL,
    [Status]           NVARCHAR (50)  NULL,
    [Subject]          NVARCHAR (400) NULL,
    [Type]             NVARCHAR (50)  NULL,
    CONSTRAINT [PK_OpenIddictAuthorizations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[OpenIddictApplications] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type]
    ON [dbo].[OpenIddictAuthorizations]([ApplicationId] ASC, [Status] ASC, [Subject] ASC, [Type] ASC);

