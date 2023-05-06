USE [ARM_CORE]
GO

/****** Object:  StoredProcedure [dbo].[SP_Bulk_Insert_Update_Users]    Script Date: 5/3/2023 2:14:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetAHarvester]
    @Type nvarchar(20),
	@Cadence INT
AS
BEGIN
  
   SELECT Item  FROM dbo.A_Harvester WHERE Action = @Type AND [Count]>0 AND @Cadence%Cadence = 0;

END
GO


