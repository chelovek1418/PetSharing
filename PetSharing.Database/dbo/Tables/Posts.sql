CREATE TABLE [dbo].[Posts] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [PetId]     INT            NOT NULL,
    [Img]       NVARCHAR (MAX) NULL,
    [Text]      NVARCHAR (MAX) NULL,
    [Date]      DATETIME2 (7)  NULL,
    [LikeCount] INT            NOT NULL,
    [IsDeleted] BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Posts_PetProfiles_PetId] FOREIGN KEY ([PetId]) REFERENCES [dbo].[PetProfiles] ([Id]) ON DELETE CASCADE
);








GO
CREATE NONCLUSTERED INDEX [IX_Posts_PetId]
    ON [dbo].[Posts]([PetId] ASC);

