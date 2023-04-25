USE ARM_CORE
GO
CREATE PROCEDURE [dbo].[SP_InsertADErrorLog]
    @ErrorMsg nvarchar(max),
	@Even nvarchar(255)
AS
BEGIN
INSERT INTO dbo.AD_ErrorLog
(
    ErrorMsg,
    Even,
    ErrorDate
)
VALUES
(   @ErrorMsg, -- ErrorMsg - varchar(max)
    @Even, -- Even - varchar(250)
    GETDATE()  -- ErrorDate - varchar(520)
    );
END
