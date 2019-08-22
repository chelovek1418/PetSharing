CREATE TABLE [dbo].[Subscriptions] (
    [UserId] INT NOT NULL,
    [PetId]  INT NOT NULL,
    CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED ([UserId] ASC, [PetId] ASC),
    CONSTRAINT [FK_Subscriptions_PetProfiles_PetId] FOREIGN KEY ([PetId]) REFERENCES [dbo].[PetProfiles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Subscriptions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Subscriptions_PetId]
    ON [dbo].[Subscriptions]([PetId] ASC);

