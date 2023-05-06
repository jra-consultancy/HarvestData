USE [ARM_CORE]
GO

/****** Object:  Table [dbo].[AD_Harvester]    Script Date: 5/3/2023 9:41:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[A_HarvesterResults](
	[HarvesterResultId] BIGINT IDENTITY(1,1) NOT NULL,
	[Item] [NVARCHAR](550) NOT NULL,
	[Property] [NVARCHAR](250) NULL,
	[Value] [NVARCHAR](250) NULL,
	[CreateDate] DATETIME null
) ON [PRIMARY]

GO

CREATE TYPE [dbo].[A_HarvesterResultsType] AS TABLE (
	[Item] [NVARCHAR](550) NOT NULL,
	[IsPingSuccess] [BIT] NULL,
	[IsWMISuccess] [BIT] NULL,
	[Property] [NVARCHAR](250) NULL,
	[Value] [NVARCHAR](250) NULL
)

GO
