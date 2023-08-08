
/****** Object:  Table [dbo].[TMP_Asset_AD]    Script Date: 3/30/2023 2:47:51 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TMP_Asset_AD]') AND type in (N'U'))
DROP TABLE [dbo].[TMP_Asset_AD]
GO

/****** Object:  Table [dbo].[AD_Assets]    Script Date: 3/30/2023 2:47:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TMP_Asset_AD](
	[AssetID] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](255) NULL,
	[DNSHostName] [nvarchar](255) NULL,
	[Enabled] [bit] NULL,
	[EduDeviceType] [nvarchar](255) NULL,
	[Created] [datetime] NULL,
	[IPv4Address] [nvarchar](15) NULL,
	[IPv6Address] [nvarchar](50) NULL,
	[isDeleted] [bit] NULL,
	[LastLogonDate] [datetime] NULL,
	[Location] [nvarchar](255) NULL,
	[LockedOut] [bit] NULL,
	[logonCount] [int] NULL,
	[ManagedBy] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[OperatingSystem] [nvarchar](255) NULL,
	[OperatingSystemVersion] [nvarchar](255) NULL,
	[PasswordExpired] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[ObjectGUID] [nvarchar](255) NULL,
	[DistinguishedName] [nvarchar](255) NULL,
	[OperatingSystemServicePack] [nvarchar](255) NULL,
	[WhenCreated] [datetime] NULL,
	[WhenChanged] [datetime] NULL,
	[ServicePrincipalName] [nvarchar](255) NULL,
	[MemberOf] [nvarchar](255) NULL,
	[UserAccountControl] [nvarchar](255) NULL,
	[OU] [nvarchar](250) NULL,
 CONSTRAINT [PK_TMP_Asset_AD] PRIMARY KEY CLUSTERED 
(
	[AssetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


