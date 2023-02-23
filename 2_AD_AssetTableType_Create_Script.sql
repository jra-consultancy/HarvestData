

/****** Object:  UserDefinedTableType [dbo].[AssetTableType]    Script Date: 2/23/2023 3:44:07 PM ******/
CREATE TYPE [dbo].[AD_AssetTableType] AS TABLE(
	[AssetID] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[DisplayName] [nvarchar](255) NULL,
	[DNSHostName] [nvarchar](255) NULL,
	[Enabled] [bit] NULL,
	[EduDeviceType] [nvarchar](255) NULL,
	[Created] [datetime2](7) NULL,
	[IPv4Address] [nvarchar](15) NULL,
	[IPv6Address] [nvarchar](255) NULL,
	[isDeleted] [bit] NULL,
	[LastLogonDate] [datetime2](7) NULL,
	[Location] [nvarchar](255) NULL,
	[LockedOut] [bit] NULL,
	[logonCount] [int] NULL,
	[ManagedBy] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[OperatingSystem] [nvarchar](255) NULL,
	[OperatingSystemVersion] [nvarchar](255) NULL,
	[PasswordExpired] [nvarchar](255) NULL
)
GO


