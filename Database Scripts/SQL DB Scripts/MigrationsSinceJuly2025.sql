USE [TradeLicenseSystem]
GO
ALTER TABLE [dbo].[Users] DROP COLUMN [Password]
ALTER TABLE [dbo].[InspectionRequests] ADD [InspectionFinalDate] [datetime]
INSERT INTO [dbo].[Status]
           ([StatusName]
           ,[StatusTypeId]
           ,[StatusDescription]
           ,[StatusKey]
           ,[IsActive]
           ,[IsDeleted]
           ,[IsLocked]
           ,[CreatedByUserId]
           ,[CreatedDateTime]
           ,[ModifiedByUserId]
           ,[ModifiedDateTime])
     VALUES
           ('Inspection Overduez',
           1,
           'Inspection Overduez',
            'InspectionOverduez',
           1,
           0,
           0,
           NULL,
           '2025-07-08 10:25:47.373',
           NULL,
           '2025-07-08 10:25:47.373')

 INSERT INTO [dbo].[NumOfDays]
          ( [Name]
     ,[NumberOfDays]
     ,[IsActive]
     ,[IsDeleted]
     ,[IsLocked]
     ,[CreatedByUserId]
     ,[CreatedDateTime]
     ,[ModifiedByUserId]
     ,[ModifiedDateTime])
    VALUES
          ('NoInspectionResponseRefusalDays',
          21,
          1,
          0,
          0,
          NULL,
          '2025-07-08 10:25:47.373',
          NULL,
          '2025-07-08 10:25:47.373')
GO


