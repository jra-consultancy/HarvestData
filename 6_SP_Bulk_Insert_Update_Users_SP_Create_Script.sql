IF OBJECT_ID('SP_Bulk_Insert_Update_Users', 'P') IS NOT NULL
BEGIN
DROP PROCEDURE [dbo].[SP_Bulk_Insert_Update_Users]
END

IF TYPE_ID('AD_UserTableType') IS NOT NULL
BEGIN
    DROP TYPE [dbo].[AD_UserTableType];
END
GO

CREATE TYPE [dbo].[AD_UserTableType] AS TABLE(
	[UserId] [nvarchar](MAX) NOT NULL,
	[AccountExpirationDate] [nvarchar](MAX) NULL,
	[CO] [nvarchar](MAX) NULL,
	[Company] [nvarchar](MAX) NULL,
	[CreateTimeStamp] [nvarchar](MAX) NULL,
	[Department] [nvarchar](MAX) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](MAX) NULL,
	[EmailAddress] [nvarchar](MAX) NULL,
	[EmployeeID] [nvarchar](MAX) NULL,
	[Enabled] [nvarchar](MAX) NULL,
	[GivenName] [nvarchar](MAX) NULL,
	[LastLogonDate] [nvarchar](MAX) NULL,
	[logonCount] [nvarchar](MAX) NOT NULL,
	[mailNickname] [nvarchar](MAX) NULL,
	[manager] [nvarchar](MAX) NULL,
	[PasswordExpired] [nvarchar](MAX) NOT NULL,
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


CREATE PROCEDURE [dbo].[SP_Bulk_Insert_Update_Users]
    @Data dbo.AD_UserTableType READONLY
AS
BEGIN
	
	IF NOT EXISTS(SELECT TOP 1* FROM @Data)
	BEGIN
		RETURN -1;
	END

	IF OBJECT_ID('TMP_User_AD', 'U') IS NOT NULL
	BEGIN
		DROP TABLE [dbo].[TMP_User_AD];
	END

	CREATE TABLE [dbo].[TMP_User_AD](
	[UserId] [nvarchar](255) NOT NULL,
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
	[SamAccountName] [nvarchar](255) NULL,
	[StreetAddress] [nvarchar](max) NULL,
	[CountryCode] [nvarchar](50) NULL,
	[OU] [nvarchar](250) NULL,
 CONSTRAINT [PK_TMP_User_AD] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

INSERT INTO TMP_User_AD (
    UserId,
    AccountExpirationDate,
    CO,
    Company,
    CreateTimeStamp,
    Department,
    Description,
    DisplayName,
    EmailAddress,
    EmployeeID,
    Enabled,
    GivenName,
    LastLogonDate,
    logonCount,
    mailNickname,
    manager,
    PasswordExpired,
    PhysicalDeliveryOfficeName,
    postalCode,
    Surname,
    TelephoneNumber,
    Title,
    UserAccountControl,
    CreatedDate,
    UpdateDate,
    SamAccountName,
    StreetAddress,
    CountryCode,
    OU
)
SELECT
    TRY_CAST(sc.UserId AS NVARCHAR(255)) AS UserId,
    TRY_CAST(sc.AccountExpirationDate AS DATETIME) AS AccountExpirationDate,
    TRY_CAST(sc.CO AS NVARCHAR(255)) AS CO,
    TRY_CAST(sc.Company AS NVARCHAR(255)) AS Company,
    TRY_CAST(sc.CreateTimeStamp AS DATETIME) AS CreateTimeStamp,
    TRY_CAST(sc.Department AS NVARCHAR(255)) AS Department,
    TRY_CAST(sc.Description AS NVARCHAR(MAX)) AS Description,
    TRY_CAST(sc.DisplayName AS NVARCHAR(255)) AS DisplayName,
    TRY_CAST(sc.EmailAddress AS NVARCHAR(255)) AS EmailAddress,
    TRY_CAST(sc.EmployeeID AS NVARCHAR(50)) AS EmployeeID,
    ISNULL(TRY_CAST(sc.Enabled AS BIT),0) AS Enabled,
    TRY_CAST(sc.GivenName AS NVARCHAR(255)) AS GivenName,
    TRY_CAST(sc.LastLogonDate AS DATETIME) AS LastLogonDate,
    ISNULL(TRY_CAST(sc.logonCount AS INT),0) AS logonCount,
    TRY_CAST(sc.mailNickname AS NVARCHAR(255)) AS mailNickname,
    TRY_CAST(sc.manager AS NVARCHAR(255)) AS manager,
    ISNULL(TRY_CAST(sc.PasswordExpired AS BIT),0) AS PasswordExpired,
    TRY_CAST(sc.PhysicalDeliveryOfficeName AS NVARCHAR(255)) AS PhysicalDeliveryOfficeName,
    TRY_CAST(sc.postalCode AS NVARCHAR(255)) AS postalCode,
    TRY_CAST(sc.Surname AS NVARCHAR(255)) AS Surname,
    TRY_CAST(sc.TelephoneNumber AS NVARCHAR(50)) AS TelephoneNumber,
    TRY_CAST(sc.Title AS NVARCHAR(255)) AS Title,
    TRY_CAST(sc.UserAccountControl AS NVARCHAR(255)) AS UserAccountControl,
    GETDATE() AS CreatedDate,
    NULL  AS UpdateDate,
    TRY_CAST(sc.SamAccountName AS NVARCHAR(255)) AS SamAccountName,
    TRY_CAST(sc.StreetAddress AS NVARCHAR(MAX)) AS StreetAddress,
    TRY_CAST(sc.CountryCode AS NVARCHAR(50)) AS CountryCode,
    TRY_CAST(sc.OU AS NVARCHAR(250)) AS OU
FROM @Data AS sc;
	
       
END
