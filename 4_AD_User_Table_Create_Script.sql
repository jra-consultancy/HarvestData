

CREATE TABLE [dbo].[AD_User](
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
	[postalCode] [nvarchar](255) NULL,
	[Surname] [nvarchar](255) NULL,
	[TelephoneNumber] [nvarchar](50) NULL,
	[Title] [nvarchar](255) NULL,
	[UserAccountControl] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


