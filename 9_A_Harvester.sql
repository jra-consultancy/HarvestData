
CREATE TABLE [dbo].[A_Harvester]
(
[HarvesterID] BIGINT NOT NULL IDENTITY(1, 1),
[Item] [nvarchar] (550) NULL,
[Type] [VARCHAR] (10) NULL,
[Action] [VARCHAR] (10) NULL,
[Count] INT NOT NULL DEFAULT(1),
[Cadence] INT NOT NULL DEFAULT(1),
[DtCreate] [DATETIME] NULL
) ON [PRIMARY]
GO


