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

SELECT 'tobe dev'

END 
END

