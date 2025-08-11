CREATE TABLE [dbo].[OpenIddictTokens] (
    [Id]               NVARCHAR (450) NOT NULL,
    [ApplicationId]    NVARCHAR (450) NULL,
    [AuthorizationId]  NVARCHAR (450) NULL,
    [ConcurrencyToken] NVARCHAR (50)  NULL,
    [CreationDate]     DATETIME2 (7)  NULL,
    [ExpirationDate]   DATETIME2 (7)  NULL,
    [Payload]          NVARCHAR (MAX) NULL,
    [Properties]       NVARCHAR (MAX) NULL,
    [RedemptionDate]   DATETIME2 (7)  NULL,
    [ReferenceId]      NVARCHAR (100) NULL,
    [Status]           NVARCHAR (50)  NULL,
    [Subject]          NVARCHAR (400) NULL,
    [Type]             NVARCHAR (50)  NULL,
    CONSTRAINT [PK_OpenIddictTokens] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OpenIddictTokens_OpenIddictApplications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[OpenIddictApplications] ([Id]),
    CONSTRAINT [FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId] FOREIGN KEY ([AuthorizationId]) REFERENCES [dbo].[OpenIddictAuthorizations] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_OpenIddictTokens_ApplicationId_Status_Subject_Type]
    ON [dbo].[OpenIddictTokens]([ApplicationId] ASC, [Status] ASC, [Subject] ASC, [Type] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OpenIddictTokens_AuthorizationId]
    ON [dbo].[OpenIddictTokens]([AuthorizationId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_OpenIddictTokens_ReferenceId]
    ON [dbo].[OpenIddictTokens]([ReferenceId] ASC) WHERE ([ReferenceId] IS NOT NULL);

