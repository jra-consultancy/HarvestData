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

DELETE a FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE (b.IsPingSuccess = 1 AND a.Action = @Type)

UPDATE a SET a.Count = a.Count - 1 FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE a.Action = @Type

END
ELSE
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

DELETE a FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE (b.IsWMISuccess = 1 AND a.Action <> 'Ping')

UPDATE a SET a.Count = a.Count - 1 FROM dbo.A_Harvester a
JOIN @Data b ON a.Item = b.Item 
WHERE a.Action <> 'Ping'

END 
END

