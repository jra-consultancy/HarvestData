IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_UpdateAHarvesterResult]') AND type in (N'P'))
DROP PROCEDURE [dbo].[SP_UpdateAHarvesterResult]
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

