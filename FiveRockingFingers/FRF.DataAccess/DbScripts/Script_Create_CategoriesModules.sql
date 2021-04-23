CREATE TABLE [dbo].[CategoriesModules](
	[ModuleId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL
) ON [PRIMARY]

ALTER TABLE [dbo].[CategoriesModules]  WITH CHECK ADD  CONSTRAINT [FK_CategoriesModules_Categories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])

ALTER TABLE [dbo].[CategoriesModules] CHECK CONSTRAINT [FK_CategoriesModules_Categories]

ALTER TABLE [dbo].[CategoriesModules]  WITH CHECK ADD  CONSTRAINT [FK_CategoriesModules_Modules] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Modules] ([Id])

ALTER TABLE [dbo].[CategoriesModules] CHECK CONSTRAINT [FK_CategoriesModules_Modules]
