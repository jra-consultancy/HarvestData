CREATE PROCEDURE SP_Bulk_Insert_Update_Users
    @UserTableType dbo.AD_UserTableType READONLY
AS
BEGIN
    MERGE dbo.[AD_User] AS target
    USING @UserTableType AS source
    ON (target.UserId = source.UserId)
    WHEN MATCHED THEN
        UPDATE SET
            target.AccountExpirationDate = source.AccountExpirationDate,
            target.CO = source.CO,
            target.Company = source.Company,
            target.CreateTimeStamp = source.CreateTimeStamp,
            target.Department = source.Department,
            target.Description = source.Description,
            target.DisplayName = source.DisplayName,
            target.EmailAddress = source.EmailAddress,
            target.EmployeeID = source.EmployeeID,
            target.Enabled = source.Enabled,
            target.GivenName = source.GivenName,
            target.LastLogonDate = source.LastLogonDate,
            target.logonCount = source.logonCount,
            target.mailNickname = source.mailNickname,
            target.manager = source.manager,
            target.PasswordExpired = source.PasswordExpired,
            target.PhysicalDeliveryOfficeName = source.PhysicalDeliveryOfficeName,
            target.postalCode = source.postalCode,
            target.Surname = source.Surname,
            target.TelephoneNumber = source.TelephoneNumber,
            target.Title = source.Title,
            target.UserAccountControl = source.UserAccountControl
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            UserId,
            AccountExpirationDate,
            CO,
            Company,
            CreateTimeStamp,
            Department,
            Description,
            DisplayName,
            EmailAddress,
            EmployeeID,
            Enabled,
            GivenName,
            LastLogonDate,
            logonCount,
            mailNickname,
            manager,
            PasswordExpired,
            PhysicalDeliveryOfficeName,
            postalCode,
            Surname,
            TelephoneNumber,
            Title,
            UserAccountControl
        )
        VALUES (
            source.UserId,
            source.AccountExpirationDate,
            source.CO,
            source.Company,
            source.CreateTimeStamp,
            source.Department,
            source.Description,
            source.DisplayName,
            source.EmailAddress,
            source.EmployeeID,
            source.Enabled,
            source.GivenName,
            source.LastLogonDate,
            source.logonCount,
            source.mailNickname,
            source.manager,
            source.PasswordExpired,
            source.PhysicalDeliveryOfficeName,
            source.postalCode,
            source.Surname,
            source.TelephoneNumber,
            source.Title,
            source.UserAccountControl
        );
END