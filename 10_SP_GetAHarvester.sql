IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_GetAHarvester]') AND type in (N'P'))
DROP PROCEDURE [dbo].[SP_GetAHarvester]
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