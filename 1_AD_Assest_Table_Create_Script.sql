

CREATE TABLE [dbo].[AD_Assets](
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
 CONSTRAINT [PK_AD_Assets] PRIMARY KEY CLUSTERED 
(
	[AssetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


