

/****** Object:  UserDefinedTableType [dbo].[AD_UserTableType]    Script Date: 3/30/2023 2:48:26 PM ******/
DROP TYPE [dbo].[AD_UserTableType]
GO

/****** Object:  UserDefinedTableType [dbo].[AD_UserTableType]    Script Date: 3/30/2023 2:48:26 PM ******/
CREATE TYPE [dbo].[AD_UserTableType] AS TABLE(
	[UserId] [nvarchar](MAX) NOT NULL,
	[AccountExpirationDate] [datetime] NULL,
	[CO] [nvarchar](MAX) NULL,
	[Company] [nvarchar](MAX) NULL,
	[CreateTimeStamp] [datetime] NULL,
	[Department] [nvarchar](MAX) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](MAX) NULL,
	[EmailAddress] [nvarchar](MAX) NULL,
	[EmployeeID] [nvarchar](MAX) NULL,
	[Enabled] [bit] NOT NULL,
	[GivenName] [nvarchar](MAX) NULL,
	[LastLogonDate] [datetime] NULL,
	[logonCount] [int] NOT NULL,
	[mailNickname] [nvarchar](MAX) NULL,
	[manager] [nvarchar](MAX) NULL,
	[PasswordExpired] [bit] NOT NULL,
	[PhysicalDeliveryOfficeName] [nvarchar](MAX) NULL,
	[postalCode] [nvarchar](MAX) NULL,
	[Surname] [nvarchar](MAX) NULL,
	[TelephoneNumber] [nvarchar](MAX) NULL,
	[Title] [nvarchar](MAX) NULL,
	[UserAccountControl] [nvarchar](MAX) NULL,
	[SamAccountName] [nvarchar](MAX) NULL,
	[StreetAddress] [nvarchar](max) NULL,
	[CountryCode] [nvarchar](MAX) NULL,
	[OU] [nvarchar](MAX) NULL
)
GO


