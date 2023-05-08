IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[A_Harvester]') AND type IN (N'U'))
DROP TABLE [dbo].[A_Harvester]
GO
CREATE TABLE [dbo].[A_Harvester]
(
[HarvesterID] BIGINT NOT NULL IDENTITY(1, 1),
[Item] [nvarchar] (550) NULL,
[Type] [VARCHAR] (100) NULL,
[Action] [VARCHAR] (100) NULL,
[Count] INT NOT NULL DEFAULT(1),
[Cadence] INT NOT NULL DEFAULT(1),
[DtCreate] [DATETIME] NULL
) ON [PRIMARY]
GO


