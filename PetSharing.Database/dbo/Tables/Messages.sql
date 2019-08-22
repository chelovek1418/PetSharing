CREATE TABLE [dbo].[Messages] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [SenderId]   INT            NOT NULL,
    [UserId]     NVARCHAR (450) NULL,
    [ReceiverId] INT            NOT NULL,
    [Text]       NVARCHAR (MAX) NULL,
    [Date]       DATETIME2 (7)  NOT NULL,
    [IsDeleted]  BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Messages_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);








GO
CREATE NONCLUSTERED INDEX [IX_Messages_UserId]
    ON [dbo].[Messages]([UserId] ASC);

