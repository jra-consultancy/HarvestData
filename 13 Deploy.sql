USE ARM_CORE
------ Log 
IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HarvestLog')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HarvestLog', -- PropertyName - nvarchar(255)
    'Log\', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'Path for log file', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO
 
IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'SPInsertHarvestLog')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'SPInsertHarvestLog', -- PropertyName - nvarchar(255)
    'EXEC dbo.SP_InsertADErrorLog @ErrorMsg,@Event', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'SP for Insert Log file ', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 

-- AD data 

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'AD_Domain')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'AD_Domain', -- PropertyName - nvarchar(255)
    '', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'Domain For AD in Harvest', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HAFilter')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HAFilter', -- PropertyName - nvarchar(255)
    '(&(objectCategory=computer)(!userAccountControl:1.2.840.113556.1.4.803:=2))', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'Filter for AD Asset Query in Harvest', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HAQuery')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HAQuery', -- PropertyName - nvarchar(255)
    '
[
  {"query": "cn", "ColumnName": "cn", "datatype": "String"},
  {"query": "whenCreated", "ColumnName": "whenCreated", "datatype": "DateTime"},
  {"query": "description", "ColumnName": "description", "datatype": "String"},
  {"query": "displayName", "ColumnName": "displayName", "datatype": "String"},
  {"query": "dNSHostName", "ColumnName": "dNSHostName", "datatype": "String"},
  {"query": "userAccountControl", "ColumnName": "userAccountControl", "datatype": "String"},
  {"query": "eucDeviceType", "ColumnName": "eucDeviceType", "datatype": "String"},
  {"query": "ipv4Address", "ColumnName": "ipv4Address", "datatype": "String"},
  {"query": "ipv6Address", "ColumnName": "ipv6Address", "datatype": "String"},
  {"query": "isDeleted", "ColumnName": "isDeleted", "datatype": "String"},
  {"query": "lastLogonTimestamp", "ColumnName": "Heartbeat", "datatype": "DateTime"},
  {"query": "location", "ColumnName": "location", "datatype": "String"},
  {"query": "lockoutTime", "ColumnName": "lockoutTime", "datatype": "String"},
  {"query": "logonCount", "ColumnName": "logonCount", "datatype": "Int32"},
  {"query": "managedBy", "ColumnName": "managedBy", "datatype": "String"},
  {"query": "name", "ColumnName": "name", "datatype": "String"},
  {"query": "operatingSystem", "ColumnName": "operatingSystem", "datatype": "String"},
  {"query": "operatingSystemVersion", "ColumnName": "operatingSystemVersion", "datatype": "String"},
  {"query": "pwdLastSet", "ColumnName": "pwdLastSet", "datatype": "String"},
  {"query": "objectGUID", "ColumnName": "objectGUID", "datatype": "String"},
  {"query": "distinguishedName", "ColumnName": "distinguishedName", "datatype": "String"},
  {"query": "operatingSystemServicePack", "ColumnName": "operatingSystemServicePack", "datatype": "String"},
  {"query": "whenChanged", "ColumnName": "whenChanged", "datatype": "DateTime"},
  {"query": "servicePrincipalName", "ColumnName": "servicePrincipalName", "datatype": "String"},
  {"query": "memberOf", "ColumnName": "memberOf", "datatype": "String"},
  {"query": "OU", "ColumnName": "OU", "datatype": "String"},
  {"query": "Enabled", "ColumnName": "Enabled", "datatype": "Int32"}
]
', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'AD Asset Query in Harvest', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HUFilter')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HUFilter', -- PropertyName - nvarchar(255)
    '(&(objectCategory=person)(objectClass=user)(!samaccountname=Administrator)(!samaccountname=SYSTEM)(!description=Built-in account for administering the computer/domain)(!userAccountControl:1.2.840.113556.1.4.803:=2))', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'Filter for AD User Query in Harvest', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HUQuery')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HUQuery', -- PropertyName - nvarchar(255)
    '
[
  {"query": "userPrincipalName", "ColumnName": "userPrincipalName", "datatype": "String"},
  {"query": "AccountExpirationDate", "ColumnName": "AccountExpirationDate", "datatype": "DateTime"},
  {"query": "givenName", "ColumnName": "givenName", "datatype": "String"},
  {"query": "company", "ColumnName": "company", "datatype": "String"},
  {"query": "lastLogonTimestamp", "ColumnName": "lastLogonTimestamp", "datatype": "DateTime"},
  {"query": "department", "ColumnName": "department", "datatype": "String"},
  {"query": "description", "ColumnName": "description", "datatype": "String"},
  {"query": "displayName", "ColumnName": "displayName", "datatype": "String"},
  {"query": "mail", "ColumnName": "mail", "datatype": "String"},
  {"query": "employeeID", "ColumnName": "employeeID", "datatype": "String"},
  {"query": "enabled", "ColumnName": "enabled", "datatype": "Int32"},
  {"query": "uSNCreated", "ColumnName": "uSNCreated", "datatype": "String"},
  {"query": "logonCount", "ColumnName": "logonCount", "datatype": "Int32"},
  {"query": "mailNickname", "ColumnName": "mailNickname", "datatype": "String"},
  {"query": "manager", "ColumnName": "manager", "datatype": "String"},
  {"query": "PasswordExpired", "ColumnName": "PasswordExpired", "datatype": "String"},
  {"query": "physicalDeliveryOfficeName", "ColumnName": "physicalDeliveryOfficeName", "datatype": "String"},
  {"query": "postalCode", "ColumnName": "postalCode", "datatype": "String"},
  {"query": "sn", "ColumnName": "sn", "datatype": "String"},
  {"query": "telephoneNumber", "ColumnName": "telephoneNumber", "datatype": "String"},
  {"query": "title", "ColumnName": "title", "datatype": "String"},
  {"query": "userAccountControl", "ColumnName": "userAccountControl", "datatype": "String"},
  {"query": "sAMAccountName", "ColumnName": "sAMAccountName", "datatype": "String"},
  {"query": "streetAddress", "ColumnName": "streetAddress", "datatype": "String"},
  {"query": "countryCode", "ColumnName": "countryCode", "datatype": "String"},
  {"query": "distinguishedName", "ColumnName": "distinguishedName", "datatype": "String"},
  {"query": "OU", "ColumnName": "OU", "datatype": "String"},
  {"query": "whenCreated", "ColumnName": "whenCreated", "datatype": "DateTime"}
]
', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'AD User Query on Harvest', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HSpForInsertAssetAD')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HSpForInsertAssetAD', -- PropertyName - nvarchar(255)
    'SP_Bulk_Insert_Update_Assets', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'SP for Insert Asset Data  ', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 
   
IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HSpForInsertUserAD')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HSpForInsertUserAD', -- PropertyName - nvarchar(255)
    'SP_Bulk_Insert_Update_Users', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'SP for Insert User AD data  ', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 

----- WIMI And Waranty

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HSpHarvestResult')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HSpHarvestResult', -- PropertyName - nvarchar(255)
    'SP_UpdateAHarvesterResult', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'SP for Insert WMI and Ping data  ', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 


IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HSpGetAHarvester')
BEGIN
INSERT INTO dbo.SystemGlobalProperties
(
    PropertyName,
    PropertyValue,
    PropertyGroup,
    PropertyNote,
    PropertyType
)
VALUES
(   'HSpGetAHarvester', -- PropertyName - nvarchar(255)
    'SP_GetAHarvester', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'SP for get WMI and Ping data  ', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO 




--------------------------------   SP and table  ------------------------------------------------------------------ 
GO 


--------------------------------  loging   ------------------------------------------------------------------ 
IF OBJECT_ID('SP_InsertADErrorLog', 'P') IS NOT NULL
BEGIN
DROP PROCEDURE [dbo].[SP_InsertADErrorLog]
END
IF OBJECT_ID('AD_ErrorLog', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AD_ErrorLog](
	[ErrorLogId] [int] IDENTITY(1,1) NOT NULL,
	[ErrorMsg] [varchar](max) NULL,
	[Event] [varchar](250) NULL,
	[ErrorDate] DATETIME NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

CREATE PROCEDURE [dbo].[SP_InsertADErrorLog]
    @ErrorMsg NVARCHAR(MAX),
	@Event NVARCHAR(255)
AS
BEGIN
INSERT INTO dbo.AD_ErrorLog
(
    ErrorMsg,
    Event,
    ErrorDate
)
VALUES
(   @ErrorMsg, -- ErrorMsg - varchar(max)
    @Event, -- Even - varchar(250)
    GETDATE()  -- ErrorDate - varchar(520)
    );
END
GO


--------------------------------  User data   ------------------------------------------------------------------ 

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
	userPrincipalName NVARCHAR(MAX) NULL,
    AccountExpirationDate NVARCHAR(MAX) NULL,
    givenName NVARCHAR(MAX) NULL,
    company NVARCHAR(MAX) NULL,
    lastLogonTimestamp NVARCHAR(MAX) NULL,
    department NVARCHAR(MAX) NULL,
    description NVARCHAR(MAX) NULL,
    displayName NVARCHAR(MAX) NULL,
    mail NVARCHAR(MAX) NULL,
    employeeID NVARCHAR(MAX) NULL,
    enabled NVARCHAR(MAX) NULL,
    uSNCreated NVARCHAR(MAX) NULL,
    logonCount NVARCHAR(MAX) NULL,
    mailNickname NVARCHAR(MAX) NULL,
    manager NVARCHAR(MAX) NULL,
    PasswordExpired NVARCHAR(MAX) NULL,
    physicalDeliveryOfficeName NVARCHAR(MAX) NULL,
    postalCode NVARCHAR(MAX) NULL,
    sn NVARCHAR(MAX) NULL,
    telephoneNumber NVARCHAR(MAX) NULL,
    title NVARCHAR(MAX) NULL,
    userAccountControl NVARCHAR(MAX) NULL,
    sAMAccountName NVARCHAR(MAX) NULL,
    streetAddress NVARCHAR(MAX) NULL,
    countryCode NVARCHAR(MAX) NULL,
    distinguishedName NVARCHAR(MAX) NULL,
    OU NVARCHAR(MAX) NULL,
	whenCreated NVARCHAR(MAX) NULL
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
    TRY_CAST(sc.userPrincipalName AS NVARCHAR(255)) AS UserId,
    TRY_CAST(sc.AccountExpirationDate AS DATETIME) AS AccountExpirationDate,
    TRY_CAST(sc.countryCode AS NVARCHAR(255)) AS CO,
    TRY_CAST(sc.Company AS NVARCHAR(255)) AS Company,
    TRY_CAST(sc.whenCreated AS DATETIME) AS CreateTimeStamp,
    TRY_CAST(sc.Department AS NVARCHAR(255)) AS Department,
    TRY_CAST(sc.Description AS NVARCHAR(MAX)) AS Description,
    TRY_CAST(sc.DisplayName AS NVARCHAR(255)) AS DisplayName,
    TRY_CAST(sc.mail AS NVARCHAR(255)) AS EmailAddress,
    TRY_CAST(sc.EmployeeID AS NVARCHAR(50)) AS EmployeeID,
    ISNULL(TRY_CAST(sc.Enabled AS BIT),0) AS Enabled,
    TRY_CAST(sc.GivenName AS NVARCHAR(255)) AS GivenName,
    TRY_CAST(sc.lastLogonTimestamp AS DATETIME) AS LastLogonDate,
    ISNULL(TRY_CAST(sc.logonCount AS INT),0) AS logonCount,
    TRY_CAST(sc.mailNickname AS NVARCHAR(255)) AS mailNickname,
    TRY_CAST(sc.manager AS NVARCHAR(255)) AS manager,
    ISNULL(TRY_CAST(sc.PasswordExpired AS BIT),0) AS PasswordExpired,
    TRY_CAST(sc.PhysicalDeliveryOfficeName AS NVARCHAR(255)) AS PhysicalDeliveryOfficeName,
    TRY_CAST(sc.postalCode AS NVARCHAR(255)) AS postalCode,
    TRY_CAST(sc.sn AS NVARCHAR(255)) AS Surname,
    TRY_CAST(sc.TelephoneNumber AS NVARCHAR(50)) AS TelephoneNumber,
    TRY_CAST(sc.Title AS NVARCHAR(255)) AS Title,
    TRY_CAST(sc.UserAccountControl AS NVARCHAR(255)) AS UserAccountControl,
    GETDATE() AS CreatedDate,
    NULL  AS UpdateDate,
    TRY_CAST(sc.SamAccountName AS NVARCHAR(255)) AS SamAccountName,
    TRY_CAST(sc.StreetAddress AS NVARCHAR(MAX)) AS StreetAddress,
    TRY_CAST(sc.CountryCode AS NVARCHAR(50)) AS CountryCode,
    TRY_CAST(sc.OU AS NVARCHAR(250)) AS OU
FROM @Data AS sc WHERE sc.userPrincipalName <> '' AND sc.userPrincipalName IS NOT NULL;
	
       
END
GO 


--------------------------------  Asset data   ------------------------------------------------------------------ 

IF OBJECT_ID('SP_Bulk_Insert_Update_Assets', 'P') IS NOT NULL
BEGIN
DROP PROCEDURE [dbo].[SP_Bulk_Insert_Update_Assets]
END
IF TYPE_ID('AD_AssetTableType') IS NOT NULL
BEGIN
    DROP TYPE [dbo].[AD_AssetTableType];
END

CREATE TYPE [dbo].[AD_AssetTableType] AS TABLE(
	cn NVARCHAR(MAX) NULL,
    whenCreated NVARCHAR(MAX) NULL,
    description NVARCHAR(MAX) NULL,
    displayName NVARCHAR(MAX) NULL,
    dNSHostName NVARCHAR(MAX) NULL,
    userAccountControl NVARCHAR(MAX) NULL,
    eucDeviceType NVARCHAR(MAX) NULL,
    ipv4Address NVARCHAR(MAX) NULL,
    ipv6Address NVARCHAR(MAX) NULL,
    isDeleted NVARCHAR(MAX) NULL,
    lastLogonTimestamp NVARCHAR(MAX) NULL,
    location NVARCHAR(MAX) NULL,
    lockoutTime NVARCHAR(MAX) NULL,
    logonCount NVARCHAR(MAX) NULL,
    managedBy NVARCHAR(MAX) NULL,
    name NVARCHAR(MAX) NULL,
    operatingSystem NVARCHAR(MAX) NULL,
    operatingSystemVersion NVARCHAR(MAX) NULL,
    pwdLastSet NVARCHAR(MAX) NULL,
    objectGUID NVARCHAR(MAX) NULL,
    distinguishedName NVARCHAR(MAX) NULL,
    operatingSystemServicePack NVARCHAR(MAX) NULL,
    whenChanged NVARCHAR(MAX) NULL,
    servicePrincipalName NVARCHAR(MAX) NULL,
    memberOf NVARCHAR(MAX) NULL,
    OU NVARCHAR(MAX) NULL,
    Enabled NVARCHAR(MAX) NULL
)
GO

CREATE PROCEDURE [dbo].[SP_Bulk_Insert_Update_Assets]
    @Data dbo.AD_AssetTableType READONLY
AS
BEGIN

	IF NOT EXISTS(SELECT TOP 1 * FROM @Data)
	BEGIN
		RETURN -1;
	END

	IF OBJECT_ID('TMP_Asset_AD', 'U') IS NOT NULL
	BEGIN
	DROP TABLE [dbo].[TMP_Asset_AD];
	END

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
	--[PasswordExpired] [nvarchar](50) NULL,
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
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
	

	INSERT INTO TMP_Asset_AD (
    AssetID,
    Description,
    DisplayName,
    DNSHostName,
    Enabled,
    EduDeviceType,
    Created,
    IPv4Address,
    IPv6Address,
    isDeleted,
    LastLogonDate,
    Location,
    LockedOut,
    logonCount,
    ManagedBy,
    Name,
    OperatingSystem,
    OperatingSystemVersion,
   -- PasswordExpired,
    CreatedDate,
    UpdatedDate,
    ObjectGUID,
    DistinguishedName,
    OperatingSystemServicePack,
    WhenCreated,
    WhenChanged,
    ServicePrincipalName,
    MemberOf,
    UserAccountControl,
    OU
)
SELECT
    ad.cn AS AssetID,
    TRY_CAST(ad.Description AS NVARCHAR(MAX)) AS Description,
    TRY_CAST(ad.DisplayName AS NVARCHAR(255)) AS DisplayName,
    TRY_CAST(ad.DNSHostName AS NVARCHAR(255)) AS DNSHostName,
    TRY_CAST(ad.Enabled AS BIT) AS Enabled,
    TRY_CAST(ad.eucDeviceType AS NVARCHAR(255)) AS EduDeviceType,
    TRY_CAST(ad.whenCreated AS DATETIME) AS Created,
    TRY_CAST(ad.IPv4Address AS NVARCHAR(15)) AS IPv4Address,
    TRY_CAST(ad.IPv6Address AS NVARCHAR(50)) AS IPv6Address,
    TRY_CAST(ad.isDeleted AS BIT) AS isDeleted,
    TRY_CAST(ad.lastLogonTimestamp AS DATETIME) AS LastLogonDate,
    TRY_CAST(ad.Location AS NVARCHAR(255)) AS Location,
    TRY_CAST((CASE WHEN ad.lockoutTime IS NULL THEN 0 ELSE 1 END) AS BIT) AS LockedOut,
    ISNULL(TRY_CAST(ad.logonCount AS INT),0) AS logonCount,
    TRY_CAST(ad.ManagedBy AS NVARCHAR(255)) AS ManagedBy,
    TRY_CAST(ad.Name AS NVARCHAR(255)) AS Name,
    TRY_CAST(ad.OperatingSystem AS NVARCHAR(255)) AS OperatingSystem,
    TRY_CAST(ad.OperatingSystemVersion AS NVARCHAR(255)) AS OperatingSystemVersion,
   -- TRY_CAST(ad.PasswordExpired AS NVARCHAR(50)) AS PasswordExpired,
    GETDATE() AS CreatedDate,
    NULL  AS UpdatedDate,
    TRY_CAST(ad.ObjectGUID AS NVARCHAR(255)) AS ObjectGUID,
    TRY_CAST(ad.DistinguishedName AS NVARCHAR(255)) AS DistinguishedName,
    TRY_CAST(ad.OperatingSystemServicePack AS NVARCHAR(255)) AS OperatingSystemServicePack,
    TRY_CAST(ad.WhenCreated AS DATETIME) AS WhenCreated,
    TRY_CAST(ad.WhenChanged AS DATETIME) AS WhenChanged,
    TRY_CAST(ad.ServicePrincipalName AS NVARCHAR(255)) AS ServicePrincipalName,
    TRY_CAST(ad.MemberOf AS NVARCHAR(255)) AS MemberOf,
    TRY_CAST(ad.UserAccountControl AS NVARCHAR(255)) AS UserAccountControl,
    TRY_CAST(ad.OU AS NVARCHAR(250)) AS OU
	--INTO temp_test_asset
FROM @Data ad;

END
GO


--------------------------------  Ping And WMI data   ------------------------------------------------------------------ 

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_GetAHarvester]') AND type in (N'P'))
DROP PROCEDURE [dbo].[SP_GetAHarvester]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[A_Harvester]') AND type IN (N'U'))
DROP TABLE [dbo].[A_Harvester]
GO

CREATE TABLE [dbo].[A_Harvester]
(
[HarvesterID] BIGINT NOT NULL IDENTITY(1, 1),
[Item] [nvarchar] (550) NULL,
[Type] [VARCHAR] (100) NULL,
[Action] [VARCHAR] (100) NULL,
[Count] INT NOT NULL DEFAULT(1),
[Cadence] INT NOT NULL DEFAULT(1),
[DtCreate] [DATETIME] NULL
) ON [PRIMARY]
GO

CREATE PROCEDURE [dbo].[SP_GetAHarvester]
    @Type nvarchar(20),
	@Cadence INT
AS
BEGIN
	
	IF(@Type = 'Warranty')
	BEGIN 
	SELECT A.Item,
	ISNULL(b.Make,'-')+'|'+
	ISNULL(b.SerialNumber,'-')+'|'+
	ISNULL(b.AssetTag,'-')
	AS Action  
	FROM dbo.A_Harvester A WITH(NOLOCK)
	JOIN dbo.Asset b WITH(NOLOCK) ON a.Item = b.AssetID
	WHERE A.Action = @Type AND A.[Count]>0 AND @Cadence%A.Cadence = 0 GROUP BY  ISNULL(b.Make, '-') + '|' + ISNULL(b.SerialNumber, '-') + '|' + ISNULL(b.AssetTag, '-'),
                                                                                A.Item ORDER BY Item;
	END
	ELSE
	BEGIN 
	SELECT Item,Action  FROM dbo.A_Harvester WITH(NOLOCK) WHERE Action = @Type AND [Count]>0 AND @Cadence%Cadence = 0 GROUP BY Item,
                                                                                            Action ORDER BY Item;
	END
END
GO




IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_UpdateAHarvesterResult]') AND type in (N'P'))
DROP PROCEDURE [dbo].[SP_UpdateAHarvesterResult]
GO

IF OBJECT_ID('A_HarvesterResults', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[A_HarvesterResults](
	[HarvesterResultId] BIGINT IDENTITY(1,1) NOT NULL,
	[Item] [NVARCHAR](550) NOT NULL,
	[Property] [NVARCHAR](250) NULL,
	[Value] [NVARCHAR](250) NULL,
	[CreateDate] DATETIME null
) ON [PRIMARY]
END
GO

IF TYPE_ID('A_HarvesterResultsType') IS NULL
BEGIN
    CREATE TYPE [dbo].[A_HarvesterResultsType] AS TABLE (
	[Item] [NVARCHAR](550) NOT NULL,
	[IsPingSuccess] [BIT] NULL,
	[IsWMISuccess] [BIT] NULL,
	[Property] [NVARCHAR](250) NULL,
	[Value] [NVARCHAR](250) NULL
)
END
GO



CREATE  PROCEDURE [dbo].[SP_UpdateAHarvesterResult]
    @Data dbo.A_HarvesterResultsType READONLY,
	@Type nvarchar(20)
AS
BEGIN
IF(@Type='Ping')
BEGIN 

-- update Heartbeat and IP addrees
UPDATE a SET Heartbeat = GETDATE(),ip = b.Value FROM dbo.Asset a
JOIN @Data b ON a.AssetID = b.Item 
WHERE (b.IsPingSuccess = 1) AND b.Property = 'IpAddress';

-- update serialnumber if IsWMISuccess = 1
UPDATE a SET a.SerialNumber = b.Value FROM dbo.Asset a
JOIN @Data b ON a.AssetID = b.Item 
WHERE (b.IsWMISuccess = 1) AND b.Property = 'SerialNumber';

-- update Make (HP, DELL, Lenovo, etc) if IsWMISuccess = 1
UPDATE a SET a.Make = b.Value FROM dbo.Asset a
JOIN @Data b ON a.AssetID = b.Item 
WHERE (b.IsWMISuccess = 1) AND b.Property = 'Manufacturer';


-- insert to A_HarvesterResults for logging , and because after we have success ping we do WMI query
INSERT INTO dbo.A_HarvesterResults
(
    Item,
    Property,
    Value,
    CreateDate
)
SELECT 
	a.Item,
    a.Property,
    a.Value,
    GETDATE()
 FROM @Data a
 LEFT JOIN A_HarvesterResults b ON b.Item = a.Item AND b.Property = a.Property
 WHERE (a.IsWMISuccess = 1 OR a.IsPingSuccess = 1) AND b.HarvesterResultId IS NULL;

  
-- delete job in A_Harvester Action ping if IsPingSuccess = 1
DELETE a FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE (b.IsPingSuccess = 1 AND a.Action = @Type)


-- if IsPingSuccess = 0 update count for cheking next loop
UPDATE a SET a.Count = a.Count - 1 FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE (b.IsPingSuccess = 0 AND a.Action = @Type)

END
ELSE IF (@Type='WMI')
BEGIN

INSERT INTO dbo.A_HarvesterResults
(
    Item,
    Property,
    Value,
    CreateDate
)
SELECT 
	a.Item,
    a.Property,
    a.Value,
    GETDATE()
 FROM @Data a
 LEFT JOIN A_HarvesterResults b ON b.Item = a.Item AND b.Property = a.Property
 WHERE a.IsWMISuccess = 1 AND b.HarvesterResultId IS NULL


-- update serialnumber if IsWMISuccess = 1
UPDATE a SET a.SerialNumber = b.Value FROM dbo.Asset a
JOIN @Data b ON a.AssetID = b.Item 
WHERE (b.IsWMISuccess = 1) AND b.Property = 'SerialNumber';

-- update Make (HP, DELL, Lenovo, etc) if IsWMISuccess = 1
UPDATE a SET a.Make = b.Value FROM dbo.Asset a
JOIN @Data b ON a.AssetID = b.Item 
WHERE (b.IsWMISuccess = 1) AND b.Property = 'Manufacturer';


DELETE a FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE (b.IsWMISuccess = 1 AND a.Action = 'WMI')

UPDATE a SET a.Count = a.Count - 1 FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE a.Action = 'WMI'

END 
ELSE IF (@Type='Warranty')
BEGIN 

-- update WarrantyDate if we gat waranty date
UPDATE a SET a.WarrantyDate = b.Value FROM dbo.Asset a
JOIN @Data b ON a.AssetID = b.Item 
WHERE b.Property = 'WarrantyDate' AND b.Value IS NOT NULL AND  b.Value <> '';

-- Delete  A_Harvester if we gat waranty date
DELETE a FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE  a.Action = 'Warranty' AND b.Property = 'WarrantyDate' AND b.Value IS NOT NULL AND  b.Value <> '';

-- Updaete count if fail to get waranty date
UPDATE a SET a.Count = a.Count - 1 FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE a.Action = 'Warranty' AND b.Property = 'WarrantyDate' AND (b.Value IS NULL OR b.Value <> '');

END 

END
GO 
