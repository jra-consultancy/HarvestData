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


UPDATE a SET Heartbeat = GETDATE() FROM dbo.Asset a
JOIN @Data b ON a.AssetID = b.Item 
WHERE (b.IsPingSuccess = 1)

INSERT INTO dbo.A_Harvester
(
    Item,
    Type,
    Action,
    Count,
    Cadence,
    DtCreate
)
SELECT 
	a.Item,    -- Item - nvarchar(550)
    'Asset',    -- Type - varchar(100)
    'WMI',    -- Action - varchar(100)
    a.Count, -- Count - int
    a.Cadence, -- Cadence - int
    GETDATE()     -- DtCreate - datetime
FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE (b.IsPingSuccess = 1 AND a.Action = @Type)


DELETE a FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE (b.IsPingSuccess = 1 AND a.Action = @Type)

UPDATE a SET a.Count = a.Count - 1 FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE a.Action = @Type

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

INSERT INTO dbo.A_Harvester
(
    Item,
    Type,
    Action,
    Count,
    Cadence,
    DtCreate
)
SELECT 
c.Item,
d.Type AS Type,
'Warranty'AS  Action,
MAX(d.Count) AS Count,
MAX(d.Cadence) AS Cadence,
GETDATE() AS DtCreate
 FROM (
SELECT a.Item
FROM (SELECT Item FROM @Data WHERE IsWMISuccess = 1  GROUP BY Item) a
) c 
JOIN dbo.A_Harvester d ON d.Item = c.Item 
WHERE d.Action = 'WMI'
GROUP BY c.Item,
         d.Type

DELETE a FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE (b.IsWMISuccess = 1 AND a.Action = 'WMI')

UPDATE a SET a.Count = a.Count - 1 FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE a.Action <> 'Ping'

END 
ELSE
BEGIN 
SELECT * FROM dbo.A_Harvester
END 

END

