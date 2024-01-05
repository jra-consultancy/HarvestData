USE ARM_CORE
------ Log 
IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'Harvester_version')
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
(   'Harvester_version', -- PropertyName - nvarchar(255)
    '', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'Harvester version', -- PropertyNote - nvarchar(255)
    'String'  -- PropertyType - nvarchar(255)
    )
END
GO

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

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HATable')
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
(   'HATable', -- PropertyName - nvarchar(255)
    'TMP_ASSET_AD', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'AD Asset  Table on Harvest', -- PropertyNote - nvarchar(255)
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

IF NOT EXISTS (SELECT TOP 1 * FROM dbo.SystemGlobalProperties WHERE PropertyName = 'HUTable')
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
(   'HUTable', -- PropertyName - nvarchar(255)
    'TMP_USER_AD', -- PropertyValue - nvarchar(max)
    'Global', -- PropertyGroup - nvarchar(255)
    'AD User Table on Harvest', -- PropertyNote - nvarchar(255)
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
--- harvest currently not use SP_Bulk_Insert_Update_Users, AD_UserTableType, so its pose to be deleted
IF OBJECT_ID('SP_Bulk_Insert_Update_Users', 'P') IS NOT NULL
BEGIN
DROP PROCEDURE [dbo].[SP_Bulk_Insert_Update_Users]
END
IF TYPE_ID('AD_UserTableType') IS NOT NULL
BEGIN
    DROP TYPE [dbo].[AD_UserTableType];
END
GO


--------------------------------  Asset data   ------------------------------------------------------------------ 
--- harvest currently not use SP_Bulk_Insert_Update_Assets, AD_AssetTableType, so its pose to be deleted
IF OBJECT_ID('SP_Bulk_Insert_Update_Assets', 'P') IS NOT NULL
BEGIN
DROP PROCEDURE [dbo].[SP_Bulk_Insert_Update_Assets]
END
IF TYPE_ID('AD_AssetTableType') IS NOT NULL
BEGIN
    DROP TYPE [dbo].[AD_AssetTableType];
END

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
