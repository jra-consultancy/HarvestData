IF OBJECT_ID('SP_Bulk_Insert_Update_Assets', 'P') IS NOT NULL
BEGIN
DROP PROCEDURE [dbo].[SP_Bulk_Insert_Update_Assets]
END
GO

IF TYPE_ID('AD_AssetTableType') IS NOT NULL
BEGIN
    DROP TYPE [dbo].[AD_AssetTableType];
END

CREATE TYPE [dbo].[AD_AssetTableType] AS TABLE(
	[AssetID] [nvarchar](MAX) NOT NULL,
	[Description] [NVARCHAR](MAX) NULL,
	[DisplayName] [NVARCHAR](MAX) NULL,
	[DNSHostName] [NVARCHAR](MAX) NULL,
	[Enabled] [NVARCHAR](MAX) NULL,
	[EduDeviceType] [NVARCHAR](MAX) NULL,
	[Created] [NVARCHAR](MAX) NULL,
	[IPv4Address] [NVARCHAR](MAX) NULL,
	[IPv6Address] [NVARCHAR](MAX) NULL,
	[isDeleted] [NVARCHAR](MAX) NULL,
	[LastLogonDate] [NVARCHAR](MAX) NULL,
	[Location] [NVARCHAR](MAX) NULL,
	[LockedOut] [NVARCHAR](MAX) NULL,
	[logonCount] [NVARCHAR](MAX) NULL,
	[ManagedBy] [NVARCHAR](MAX) NULL,
	[Name] [NVARCHAR](MAX) NULL,
	[OperatingSystem] [NVARCHAR](MAX) NULL,
	[OperatingSystemVersion] [NVARCHAR](MAX) NULL,
	[PasswordExpired] [NVARCHAR](MAX) NULL,
	[ObjectGUID] [NVARCHAR](MAX) NULL,
	[DistinguishedName] [NVARCHAR](MAX) NULL,
	[OperatingSystemServicePack] [NVARCHAR](MAX) NULL,
	[WhenCreated] [NVARCHAR](MAX) NULL,
	[WhenChanged] [NVARCHAR](MAX) NULL,
	[ServicePrincipalName] [NVARCHAR](MAX) NULL,
	[MemberOf] [NVARCHAR](MAX) NULL,
	[UserAccountControl] [NVARCHAR](MAX) NULL,
	[OU] [NVARCHAR](MAX) NULL
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
    PasswordExpired,
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
    ad.AssetID,
    TRY_CAST(ad.Description AS NVARCHAR(MAX)) AS Description,
    TRY_CAST(ad.DisplayName AS NVARCHAR(255)) AS DisplayName,
    TRY_CAST(ad.DNSHostName AS NVARCHAR(255)) AS DNSHostName,
    TRY_CAST(ad.Enabled AS BIT) AS Enabled,
    TRY_CAST(ad.EduDeviceType AS NVARCHAR(255)) AS EduDeviceType,
    TRY_CAST(ad.Created AS DATETIME) AS Created,
    TRY_CAST(ad.IPv4Address AS NVARCHAR(15)) AS IPv4Address,
    TRY_CAST(ad.IPv6Address AS NVARCHAR(50)) AS IPv6Address,
    TRY_CAST(ad.isDeleted AS BIT) AS isDeleted,
    TRY_CAST(ad.LastLogonDate AS DATETIME) AS LastLogonDate,
    TRY_CAST(ad.Location AS NVARCHAR(255)) AS Location,
    TRY_CAST(ad.LockedOut AS BIT) AS LockedOut,
    TRY_CAST(ad.logonCount AS INT) AS logonCount,
    TRY_CAST(ad.ManagedBy AS NVARCHAR(255)) AS ManagedBy,
    TRY_CAST(ad.Name AS NVARCHAR(255)) AS Name,
    TRY_CAST(ad.OperatingSystem AS NVARCHAR(255)) AS OperatingSystem,
    TRY_CAST(ad.OperatingSystemVersion AS NVARCHAR(255)) AS OperatingSystemVersion,
    TRY_CAST(ad.PasswordExpired AS NVARCHAR(50)) AS PasswordExpired,
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
FROM @Data ad;

END
GO
