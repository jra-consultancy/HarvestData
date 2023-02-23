
/****** Object:  UserDefinedTableType [dbo].[UserTableType]    Script Date: 2/23/2023 3:48:24 PM ******/
CREATE TYPE [dbo].[AD_UserTableType] AS TABLE(
	[UserId] [nvarchar](50) NOT NULL,
	[AccountExpirationDate] [datetime] NULL,
	[CO] [nvarchar](255) NULL,
	[Company] [nvarchar](255) NULL,
	[CreateTimeStamp] [datetime] NULL,
	[Department] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](255) NULL,
	[EmailAddress] [nvarchar](255) NULL,
	[EmployeeID] [nvarchar](50) NULL,
	[Enabled] [bit] NOT NULL,
	[GivenName] [nvarchar](255) NULL,
	[LastLogonDate] [datetime] NULL,
	[logonCount] [int] NOT NULL,
	[mailNickname] [nvarchar](255) NULL,
	[manager] [nvarchar](255) NULL,
	[PasswordExpired] [bit] NOT NULL,
	[PhysicalDeliveryOfficeName] [nvarchar](255) NULL,
	[postalCode] [nvarchar](50) NULL,
	[Surname] [nvarchar](255) NULL,
	[TelephoneNumber] [nvarchar](50) NULL,
	[Title] [nvarchar](255) NULL,
	[UserAccountControl] [nvarchar](255) NULL
)
GO


