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
	ISNULL((SELECT TOP 1 B.Value FROM dbo.A_HarvesterResults B WITH(NOLOCK) WHERE Item = A.Item  AND B.Property ='Manufacturer'),'-')+'|'+
	ISNULL((SELECT TOP 1 B.Value FROM dbo.A_HarvesterResults B WITH(NOLOCK) WHERE Item = A.Item  AND B.Property ='SerialNumber'),'-')+'|'+
	ISNULL((SELECT TOP 1 B.Value FROM dbo.A_HarvesterResults B WITH(NOLOCK) WHERE Item = A.Item  AND B.Property ='SystemSKUNumber'),'-')
	AS Action  
	FROM dbo.A_Harvester A WITH(NOLOCK)
	WHERE A.Action = @Type AND A.[Count]>0 AND @Cadence%A.Cadence = 0 GROUP BY A.Item,Action ORDER BY Item;
	END
	ELSE
	BEGIN 
	SELECT Item,Action  FROM dbo.A_Harvester WITH(NOLOCK) WHERE Action = @Type AND [Count]>0 AND @Cadence%Cadence = 0 GROUP BY Item,
                                                                                            Action ORDER BY Item;
	END
END
GO