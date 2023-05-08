IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_InsertADErrorLog]') AND type in (N'P'))
DROP PROCEDURE [dbo].[SP_InsertADErrorLog]
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

EXEC dbo.SP_InsertADErrorLog @ErrorMsg = N'', -- nvarchar(max)
                             @Event = N''     -- nvarchar(255)
