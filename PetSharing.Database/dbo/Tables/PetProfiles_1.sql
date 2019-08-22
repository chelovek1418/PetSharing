CREATE TABLE [dbo].[PetProfiles] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX) NULL,
    [Img]           NVARCHAR (MAX) NULL,
    [Type]          NVARCHAR (MAX) NULL,
    [Breed]         NVARCHAR (MAX) NULL,
    [Gender]        INT            NULL,
    [DateOfBirth]   DATETIME2 (7)  NULL,
    [Location]      NVARCHAR (MAX) NULL,
    [AvgLikeCount]  FLOAT (53)     NOT NULL,
    [IsSale]        BIT            NOT NULL,
    [IsReadyForSex] BIT            NOT NULL,
    [IsShare]       BIT            NOT NULL,
    [OwnerId]       NVARCHAR (450) NULL,
    [IsDeleted]     BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PetProfiles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PetProfiles_AspNetUsers_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_PetProfiles_OwnerId]
    ON [dbo].[PetProfiles]([OwnerId] ASC);

