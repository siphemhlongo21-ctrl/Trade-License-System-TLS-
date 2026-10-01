use TradeLicenseSystem

--this will delete all license application data on the system
--if there is any conflicts try executing again
delete from[dbo].[Refusals]
delete from[dbo].[FileUploads]
delete from[dbo].[AdministratorReviews]
delete from[dbo].[ManagerReviews]
delete from[dbo].[Payments]
delete from[dbo].[ELMAH_Error]
delete from[dbo].[RevokeCancelReviews]
delete from[dbo].[InspectionHistories]
delete from[dbo].[InspectionAppealsReviews]
delete from[dbo].[InspectionAppeals]
delete from[dbo].[InspectionResponses]
delete from[dbo].[InspectionRequests]
delete from[dbo].[LicenseApplicationConditions]

delete from[dbo].[Licenses]
delete from[dbo].[Amendments]
delete from[dbo].[ChiefReviews]
delete from[dbo].[RevokeCancelReviews]
delete from[dbo].[EscalationMainTables]
delete from[dbo].[BusinessMangers]
delete from[dbo].[BusinessEmployees]




delete from[dbo].[ChiefReviews]
delete from[dbo].[PaymentLicenses]
delete from[dbo].[LicensesRevokeds]

delete from[dbo].[LicensesRevokeds]

delete from[dbo].[Businesses]
delete from[dbo].[Clients]

delete from [dbo].[Audits]




UPDATE [dbo].[MigratedBusinesses]
   SET 
      [IsActive] = 'True'
      ,[IsDeleted] = 'False'
     
      
 
GO
UPDATE [dbo].[MigratedClients]
   SET 
      [IsActive] = 'True'
      ,[IsDeleted] = 'False'
     
      
 
GO

