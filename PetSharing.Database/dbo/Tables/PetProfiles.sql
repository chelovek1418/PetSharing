CREATE TABLE [dbo].[PetProfiles] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX) NULL,
    [Img]           NVARCHAR (MAX) NULL,
    [Type]          NVARCHAR (MAX) NULL,
    [Breed]         NVARCHAR (MAX) NULL,
    [Gender]        INT            NOT NULL,
    [DateOfBirth]   DATETIME2 (7)  NOT NULL,
    [Location]      NVARCHAR (MAX) NULL,
    [AvgLikeCount]  FLOAT (53)     NOT NULL,
    [IsSale]        BIT            NOT NULL,
    [IsReadyForSex] BIT            NOT NULL,
    [IsShare]       BIT            NOT NULL,
    [OwnerId]       INT            NOT NULL,
    [UserId]        INT            NULL,
    CONSTRAINT [PK_PetProfiles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PetProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_PetProfiles_UserId]
    ON [dbo].[PetProfiles]([UserId] ASC);

