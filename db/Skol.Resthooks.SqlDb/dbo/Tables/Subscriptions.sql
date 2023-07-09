CREATE TABLE [dbo].[Subscriptions] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [DisplayName]     NVARCHAR (MAX)   NOT NULL,
    [Environment]     NVARCHAR (MAX)   NOT NULL DEFAULT ('Development'),
    [Topics]          NVARCHAR (MAX)   NOT NULL, -- ICollection<string> kind
    [Webhook]         NVARCHAR (MAX)   NOT NULL, -- WebhookOptions kind
    [ActivationCode]  NVARCHAR (MAX)   NOT NULL,
    [Enabled]         BIT              NOT NULL DEFAULT (0),
    [Archived]        BIT              NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
