CREATE TABLE [dbo].[ProjectModules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Alias] [nvarchar](50) NOT NULL,
	[Cost] [int] NOT NULL,
 CONSTRAINT [PK_ProjectModules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[ProjectModules]  WITH CHECK ADD  CONSTRAINT [FK_ProjectModules_Modules] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Modules] ([Id])

ALTER TABLE [dbo].[ProjectModules] CHECK CONSTRAINT [FK_ProjectModules_Modules]

ALTER TABLE [dbo].[ProjectModules]  WITH CHECK ADD  CONSTRAINT [FK_ProjectModules_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([Id])

ALTER TABLE [dbo].[ProjectModules] CHECK CONSTRAINT [FK_ProjectModules_Projects]
