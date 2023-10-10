
IF OBJECT_ID('SP_InsertADErrorLog', 'P') IS NOT NULL
BEGIN
DROP PROCEDURE [dbo].[SP_InsertADErrorLog]
END

IF OBJECT_ID('AD_ErrorLog', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AD_ErrorLog](
	[ErrorLogId] [int] IDENTITY(1,1) NOT NULL,
	[ErrorMsg] [varchar](max) NULL,
	[Event] [varchar](250) NULL,
	[ErrorDate] DATETIME NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
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

