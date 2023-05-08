
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AD_ErrorLog]') AND type IN (N'U'))
DROP TABLE [dbo].[AD_ErrorLog]
GO

/****** Object:  Table [dbo].[CORE_ErrorLog]    Script Date: 4/24/2023 2:05:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AD_ErrorLog](
	[ErrorLogId] [int] IDENTITY(1,1) NOT NULL,
	[ErrorMsg] [varchar](max) NULL,
	[Event] [varchar](250) NULL,
	[ErrorDate] DATETIME NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

