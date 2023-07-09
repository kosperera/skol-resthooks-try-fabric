CREATE TABLE [dbo].[Topics] (
    [Id]           INT            NOT NULL,
    [Name]         NVARCHAR (MAX) NOT NULL,
    [IsSystemKind] BIT            NOT NULL DEFAULT (0),
    [Archived]     BIT            NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Topics] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
