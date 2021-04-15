CREATE TABLE [dbo].[ProjectResources](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdResource] [int] NOT NULL,
	[IdProject] [int] NOT NULL,
	[DedicatedHours] [int] NOT NULL,
	[BeginDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_ProjectResources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[ProjectResources]  WITH CHECK ADD  CONSTRAINT [FK_ProjectResources_Projects] FOREIGN KEY([IdProject])
REFERENCES [dbo].[Projects] ([Id])

ALTER TABLE [dbo].[ProjectResources] CHECK CONSTRAINT [FK_ProjectResources_Projects]

ALTER TABLE [dbo].[ProjectResources]  WITH CHECK ADD  CONSTRAINT [FK_ProjectResources_Resources] FOREIGN KEY([IdResource])
REFERENCES [dbo].[Resources] ([Id])

ALTER TABLE [dbo].[ProjectResources] CHECK CONSTRAINT [FK_ProjectResources_Resources]
