IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_GetAHarvester]') AND type in (N'P'))
DROP PROCEDURE [dbo].[SP_GetAHarvester]
GO

CREATE PROCEDURE [dbo].[SP_GetAHarvester]
    @Type nvarchar(20),
	@Cadence INT
AS
BEGIN
  
	IF (@Type='Ping')
	BEGIN 
	 SELECT Item,Action  FROM dbo.A_Harvester WHERE Action = @Type AND [Count]>0 AND @Cadence%Cadence = 0 GROUP BY Item,
                                                                                                                   Action;
	END
	ELSE
    BEGIN 
	 SELECT Item,Action  FROM dbo.A_Harvester WHERE Action <> 'Ping' AND [Count]>0 AND @Cadence%Cadence = 0 GROUP BY Item,
                                                                                                                     Action ORDER BY Item;
	END

END
GO


SELECT * FROM dbo.A_HarvesterResults