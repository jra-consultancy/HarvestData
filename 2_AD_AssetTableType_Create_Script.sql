
/****** Object:  UserDefinedTableType [dbo].[AD_AssetTableType]    Script Date: 3/30/2023 2:48:15 PM ******/
DROP TYPE [dbo].[AD_AssetTableType]
GO

/****** Object:  UserDefinedTableType [dbo].[AD_AssetTableType]    Script Date: 3/30/2023 2:48:15 PM ******/
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
	[PasswordExpired] [nvarchar](255) NULL,
	[ObjectGUID] [nvarchar](255) NULL,
	[DistinguishedName] [nvarchar](255) NULL,
	[OperatingSystemServicePack] [nvarchar](255) NULL,
	[WhenCreated] [datetime] NULL,
	[WhenChanged] [datetime] NULL,
	[ServicePrincipalName] [nvarchar](255) NULL,
	[MemberOf] [nvarchar](255) NULL,
	[UserAccountControl] [nvarchar](255) NULL,
	[OU] [nvarchar](250) NULL
)
GO


