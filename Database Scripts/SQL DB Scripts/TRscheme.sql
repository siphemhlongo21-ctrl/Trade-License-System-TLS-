CREATE DATABASE TradeLicenseSystem
GO
USE [TradeLicenseSystem]
GO
/****** Object:  User [TradeUser]    Script Date: 2/11/2021 1:07:07 PM ******/
CREATE USER [TradeUser] FOR LOGIN [TradeUser] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [TradeUser]
GO
ALTER ROLE [db_datareader] ADD MEMBER [TradeUser]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [TradeUser]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_LicenseCondition]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_LicenseCondition](
	[LicenceNo] [int] NOT NULL,
	[Description] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_LicensesCentral]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_LicensesCentral](
	[RefNo] [nvarchar](50) NULL,
	[BusinessName] [varchar](max) NULL,
	[PhysicalAddress1] [nvarchar](300) NULL,
	[PhysicalAddress2] [nvarchar](300) NULL,
	[Proprietor] [nvarchar](300) NULL,
	[Employer] [nvarchar](300) NULL,
	[Telephone] [nvarchar](300) NULL,
	[Postal1] [nvarchar](300) NULL,
	[Postal2] [nvarchar](300) NULL,
	[Postal3] [nvarchar](300) NULL,
	[LicenseType] [nvarchar](300) NULL,
	[LicenseSubType] [nvarchar](300) NULL,
	[CurrentDate] [nvarchar](100) NULL,
	[LicenseIssueDate] [nvarchar](300) NULL,
	[NotificationUpdatedate] [nvarchar](300) NULL,
	[Licenceconditionno] [int] NULL,
	[Licencecondition] [nvarchar](300) NULL,
	[PendingIndicator] [bit] NULL,
	[DatePended] [nvarchar](1) NULL,
	[AppNo] [nvarchar](1) NULL,
	[TradingName] [nvarchar](300) NULL,
	[PropName] [nvarchar](300) NULL,
	[PhysicalAddress] [nvarchar](300) NULL,
	[AnnualNotification] [bit] NULL,
	[ItemNo] [nvarchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_LicenseSubType]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_LicenseSubType](
	[LicenseSubType] [nvarchar](2) NULL,
	[Description] [nvarchar](50) NULL,
	[SubCategoryId] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_LicenseTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_LicenseTypes](
	[LicenseType] [nvarchar](2) NOT NULL,
	[Description] [nvarchar](50) NULL,
	[LicenseTypeId] [int] NULL,
	[ItemTypeId] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[_StreetNames]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_StreetNames](
	[No] [float] NULL,
	[StreetName] [nvarchar](255) NULL,
	[Streetold] [nvarchar](255) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdministratorReviews]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdministratorReviews](
	[AdministratorReviewId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Decision] [nvarchar](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[DateReviewed] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.AdministratorReviews] PRIMARY KEY CLUSTERED 
(
	[AdministratorReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Amendments]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Amendments](
	[AmendmentsId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[AmendmentType] [nvarchar](50) NULL,
	[TableAmended] [nvarchar](80) NULL,
	[FieldAmended] [nvarchar](100) NULL,
	[OldData] [nvarchar](100) NULL,
	[NewData] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.Amendments] PRIMARY KEY CLUSTERED 
(
	[AmendmentsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[UserId] [int] NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Audits]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Audits](
	[AuditId] [int] IDENTITY(1,1) NOT NULL,
	[Action] [nvarchar](max) NULL,
	[PrimaryKey] [int] NOT NULL,
	[TableName] [nvarchar](max) NULL,
	[ColumnName] [nvarchar](max) NULL,
	[OriginalValue] [nvarchar](max) NULL,
	[CurrentValue] [nvarchar](max) NULL,
	[AuditByUserId] [int] NULL,
	[AuditDateTime] [datetime] NOT NULL,
	[LicenseId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Audits] PRIMARY KEY CLUSTERED 
(
	[AuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BusinessEmployees]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessEmployees](
	[BusinessEmployeeId] [int] IDENTITY(1,1) NOT NULL,
	[BusinessId] [int] NOT NULL,
	[NameOfEmployer] [nvarchar](100) NULL,
	[EmployerResidentialAddress1] [nvarchar](100) NULL,
	[EmployerResidentialAddress2] [nvarchar](100) NULL,
	[EmployerResidentialAddress3] [nvarchar](100) NULL,
	[EmployerResidentialAddressCode] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.BusinessEmployees] PRIMARY KEY CLUSTERED 
(
	[BusinessEmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Businesses]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Businesses](
	[BusinessId] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[ProposedTradeName] [nvarchar](200) NOT NULL,
	[BusinessTypeId] [int] NOT NULL,
	[PostalAddress1] [nvarchar](100) NOT NULL,
	[PostalAddress2] [nvarchar](100) NOT NULL,
	[PostalAddress3] [nvarchar](100) NULL,
	[PostalAddressCode] [int] NOT NULL,
	[ResidentialShopOrUnitNumber] [int] NULL,
	[ResidentialStreetAddress] [nvarchar](100) NULL,
	[ResidentialAddress1] [nvarchar](100) NOT NULL,
	[ResidentialAddress2] [nvarchar](100) NOT NULL,
	[ResidentialAddress3] [nvarchar](100) NULL,
	[ResidentialAddressCode] [int] NOT NULL,
	[TelephoneNumber] [nvarchar](50) NULL,
	[CellphoneNumber] [nvarchar](10) NULL,
	[FaxNumber] [nvarchar](50) NULL,
	[TitleDeedTypeId] [int] NULL,
	[OperationStructureTypeId] [int] NOT NULL,
	[BusinessStatusId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[RatesAccountNumber] [nvarchar](100) NULL,
	[ItemTypeId] [int] NULL,
	[KnownAs] [nvarchar](200) NULL,
	[SameAs] [bit] NULL,
	[AltCellphoneNumber] [nvarchar](10) NULL,
 CONSTRAINT [PK_dbo.Businesses] PRIMARY KEY CLUSTERED 
(
	[BusinessId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BusinessMangers]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessMangers](
	[BusinessMangerId] [int] IDENTITY(1,1) NOT NULL,
	[BusinessId] [int] NOT NULL,
	[NameOfBusinessOperator] [nvarchar](200) NULL,
	[OperatorIdentityOrPassportNumber] [nvarchar](50) NULL,
	[OperatorResidentialAddress1] [nvarchar](100) NULL,
	[OperatorResidentialAddress2] [nvarchar](100) NOT NULL,
	[OperatorResidentialAddress3] [nvarchar](100) NULL,
	[OperatorResidentialAddressCode] [int] NOT NULL,
	[BusinessOperatorId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.BusinessMangers] PRIMARY KEY CLUSTERED 
(
	[BusinessMangerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BusinessOperationTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessOperationTypes](
	[BusinessOperationTypeId] [int] IDENTITY(1,1) NOT NULL,
	[BusinessOperationTypeName] [nvarchar](max) NULL,
	[BusinessOperationTypeDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.BusinessOperationTypes] PRIMARY KEY CLUSTERED 
(
	[BusinessOperationTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BusinessOperators]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessOperators](
	[BusinessOperatorId] [int] IDENTITY(1,1) NOT NULL,
	[BusinessOperatorName] [nvarchar](max) NULL,
	[BusinessOperatorDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.BusinessOperators] PRIMARY KEY CLUSTERED 
(
	[BusinessOperatorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BusinessTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BusinessTypes](
	[BusinessTypeId] [int] IDENTITY(1,1) NOT NULL,
	[BusinessTypeName] [nvarchar](100) NOT NULL,
	[BusinessTypeDescription] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.BusinessTypes] PRIMARY KEY CLUSTERED 
(
	[BusinessTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChiefReviews]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiefReviews](
	[ChiefReviewId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Decision] [nvarchar](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[DateReviewed] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.ChiefReviews] PRIMARY KEY CLUSTERED 
(
	[ChiefReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Clients]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[ClientId] [int] IDENTITY(1,1) NOT NULL,
	[IdentityOrPassportNumber] [nvarchar](50) NULL,
	[Name] [nvarchar](80) NULL,
	[Surname] [nvarchar](80) NULL,
	[ResidentialAddress1] [nvarchar](100) NOT NULL,
	[ResidentialAddress2] [nvarchar](100) NOT NULL,
	[ResidentialAddress3] [nvarchar](100) NOT NULL,
	[ResidentialAddressCode] [int] NOT NULL,
	[PostalAddress1] [nvarchar](100) NOT NULL,
	[PostalAddress2] [nvarchar](100) NOT NULL,
	[PostalAddress3] [nvarchar](100) NOT NULL,
	[PostalAddressCode] [int] NOT NULL,
	[TelephoneNumber] [nvarchar](100) NULL,
	[CellphoneNumber] [nvarchar](10) NULL,
	[FaxNumber] [nvarchar](100) NULL,
	[EmailAddress] [nvarchar](100) NULL,
	[ClientStatusId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[Nationality] [nvarchar](50) NULL,
	[NationalityOther] [nvarchar](50) NULL,
	[ExpiryPermit] [nvarchar](50) NULL,
	[Individual] [nvarchar](50) NULL,
	[CustomerType] [nvarchar](50) NULL,
	[RegNumber] [nvarchar](50) NULL,
	[BusinessName] [nvarchar](200) NULL,
	[SameAs] [bit] NULL,
	[AltCellphoneNumber] [nvarchar](10) NULL,
 CONSTRAINT [PK_dbo.Clients] PRIMARY KEY CLUSTERED 
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cmn_sequences]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cmn_sequences](
	[org_process_id] [int] NOT NULL,
	[seq_id] [int] NOT NULL,
	[seq_name] [varchar](10) NULL,
	[last_value] [bigint] NULL,
	[month] [int] NULL,
	[year] [int] NULL,
	[sys_timestamp] [timestamp] NOT NULL,
	[inserted_user] [varchar](50) NULL,
	[inserted_date] [datetime] NULL,
	[updated_user] [varchar](50) NULL,
	[updated_date] [datetime] NULL,
	[hkey_id] [bigint] NULL,
 CONSTRAINT [cmn_sequences_pk] PRIMARY KEY CLUSTERED 
(
	[org_process_id] ASC,
	[seq_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Conditions]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conditions](
	[ConditionId] [int] IDENTITY(1,1) NOT NULL,
	[ConditionName] [nvarchar](max) NOT NULL,
	[ConditionDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.Conditions] PRIMARY KEY CLUSTERED 
(
	[ConditionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DepartmentContacts]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DepartmentContacts](
	[DepartmentContactId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[IsPrinciple] [bit] NOT NULL,
	[DepartmentHeadContactId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[RoleName] [nvarchar](100) NULL,
 CONSTRAINT [PK_dbo.DepartmentContacts] PRIMARY KEY CLUSTERED 
(
	[DepartmentContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Departments]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[DepartmentId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentName] [nvarchar](max) NOT NULL,
	[DepartmentDescription] [nvarchar](max) NOT NULL,
	[DepartmentKey] [nvarchar](max) NOT NULL,
	[DepartmentStructureType] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[RegionId] [int] NULL,
 CONSTRAINT [PK_dbo.Departments] PRIMARY KEY CLUSTERED 
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DepartmentServiceLevelAgreements]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DepartmentServiceLevelAgreements](
	[DepartmentSlaId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[SlaId] [int] NOT NULL,
	[SlaDays] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.DepartmentServiceLevelAgreements] PRIMARY KEY CLUSTERED 
(
	[DepartmentSlaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentCheckLists]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentCheckLists](
	[DocumentCheckListId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseTypeId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.DocumentCheckLists] PRIMARY KEY CLUSTERED 
(
	[DocumentCheckListId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Documents]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Documents](
	[DocumentId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentName] [nvarchar](100) NOT NULL,
	[DocumentDescription] [nvarchar](max) NULL,
	[DocumentKey] [nvarchar](max) NULL,
	[DocumentTypeId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.Documents] PRIMARY KEY CLUSTERED 
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentTypes](
	[DocumentTypeId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentTypeName] [nvarchar](100) NOT NULL,
	[DocumentTypeDescription] [nvarchar](max) NULL,
	[DocumentTypeKey] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.DocumentTypes] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DropdownItems]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DropdownItems](
	[DropdownItemsId] [int] IDENTITY(1,1) NOT NULL,
	[ItemTypeName] [nvarchar](max) NOT NULL,
	[ItemTypeDescription] [nvarchar](max) NULL,
	[ItemTypeKey] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.DropdownItems] PRIMARY KEY CLUSTERED 
(
	[DropdownItemsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [ntext] NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY NONCLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Enquiries]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Enquiries](
	[EnquiryId] [int] IDENTITY(1,1) NOT NULL,
	[ClientName] [nvarchar](max) NOT NULL,
	[ProposedTradeName] [nvarchar](max) NOT NULL,
	[PostalAddress1] [nvarchar](max) NULL,
	[PostalAddress2] [nvarchar](max) NULL,
	[PostalAddress3] [nvarchar](max) NULL,
	[PostalAddressCode] [int] NOT NULL,
	[EnquiryReport] [nvarchar](max) NOT NULL,
	[InspectorsReport] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[LicenseTypeId] [int] NOT NULL,
	[DepartmentId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Enquiries] PRIMARY KEY CLUSTERED 
(
	[EnquiryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EscalationLogs]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EscalationLogs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EventID] [int] NOT NULL,
	[EventDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.EscalationLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EscalationMainTables]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EscalationMainTables](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[StatusDaysOLD] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[StatusName] [nvarchar](150) NULL,
 CONSTRAINT [PK_dbo.EscalationMainTables] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FileUploads]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileUploads](
	[FileUploadId] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NULL,
	[ClientName] [nvarchar](max) NULL,
	[FileName] [nvarchar](max) NULL,
	[FilePath] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[DocumentId] [int] NOT NULL,
	[referenceId] [int] NOT NULL,
	[Comments] [nvarchar](max) NULL,
	[Uploadedby] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.FileUploads] PRIMARY KEY CLUSTERED 
(
	[FileUploadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InspectionAppeals]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InspectionAppeals](
	[InspectionAppealId] [int] IDENTITY(1,1) NOT NULL,
	[InspectionResponseId] [int] NOT NULL,
	[LicenseId] [int] NOT NULL,
	[IdentityOrPassportNumber] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Surname] [nvarchar](max) NOT NULL,
	[Individual] [nvarchar](max) NULL,
	[Nationality] [nvarchar](max) NULL,
	[NationalityOther] [nvarchar](max) NULL,
	[ExpiryPermit] [nvarchar](max) NULL,
	[ResidentialAddress1] [nvarchar](max) NOT NULL,
	[ResidentialAddress2] [nvarchar](max) NOT NULL,
	[ResidentialAddress3] [nvarchar](max) NOT NULL,
	[ResidentialAddressCode] [int] NOT NULL,
	[PostalAddress1] [nvarchar](max) NOT NULL,
	[PostalAddress2] [nvarchar](max) NOT NULL,
	[PostalAddress3] [nvarchar](max) NOT NULL,
	[PostalAddressCode] [int] NOT NULL,
	[TelephoneNumber] [nvarchar](max) NULL,
	[CellphoneNumber] [nvarchar](max) NOT NULL,
	[FaxNumber] [nvarchar](max) NULL,
	[EmailAddress] [nvarchar](max) NOT NULL,
	[Reason] [nvarchar](max) NULL,
	[AppealDateTime] [datetime] NULL,
	[StatusId] [int] NOT NULL,
	[AppealDate] [datetime] NULL,
	[AppealDocDate] [datetime] NULL,
	[SameAs] [bit] NULL,
 CONSTRAINT [PK_dbo.InspectionAppeals] PRIMARY KEY CLUSTERED 
(
	[InspectionAppealId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InspectionAppealsReviews]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InspectionAppealsReviews](
	[InspectionAppealsReviewId] [int] IDENTITY(1,1) NOT NULL,
	[InspectionAppealId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Decision] [nvarchar](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[DateReviewed] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.InspectionAppealsReviews] PRIMARY KEY CLUSTERED 
(
	[InspectionAppealsReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InspectionHistories]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InspectionHistories](
	[InspectionHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[InspectionRequestId] [int] NOT NULL,
	[InspectionResponseId] [int] NOT NULL,
	[InspectionDateTime] [datetime] NOT NULL,
	[InspectionFailureCount] [int] NOT NULL,
	[NonComplianceDateTime] [datetime] NULL,
	[ComplianceDateTime] [datetime] NULL,
	[AdhocInspector] [bit] NOT NULL,
	[LicenseId] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[StatusId] [int] NOT NULL,
	[CapturedByClerk] [bit] NOT NULL,
	[Clerk] [nvarchar](500) NULL,
 CONSTRAINT [PK_dbo.InspectionHistories] PRIMARY KEY CLUSTERED 
(
	[InspectionHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InspectionRequests]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InspectionRequests](
	[InspectionRequestId] [int] IDENTITY(1,1) NOT NULL,
	[OverrideOrderIndex] [bit] NOT NULL,
	[LicenseId] [int] NOT NULL,
	[LicenseTypeId] [int] NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[ClientId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[SlaExpiryDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[RefNumber] [nvarchar](128) NULL,
	[AllocatedDate] [datetime] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[DepartmentContactId] [int] NOT NULL,
	[AppealFinalDate] [datetime] NULL,
 CONSTRAINT [PK_dbo.InspectionRequests] PRIMARY KEY CLUSTERED 
(
	[InspectionRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InspectionResponses]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InspectionResponses](
	[InspectionResponseId] [int] IDENTITY(1,1) NOT NULL,
	[InspectionRequestId] [int] NOT NULL,
	[NonComplianceDateTime] [datetime] NULL,
	[ComplianceDateTime] [datetime] NULL,
	[LicenseId] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[StatusId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[InspectionDateTime] [datetime] NOT NULL,
	[InspectionFailureCount] [int] NOT NULL,
	[AdhocInspector] [bit] NOT NULL,
	[CapturedByClerk] [bit] NOT NULL,
	[Clerk] [nvarchar](500) NULL,
 CONSTRAINT [PK_dbo.InspectionResponses] PRIMARY KEY CLUSTERED 
(
	[InspectionResponseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemConditions]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemConditions](
	[ItemConditionId] [int] IDENTITY(1,1) NOT NULL,
	[ItemTypeId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[ItemConditionName] [nvarchar](max) NOT NULL,
	[ItemConditionDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ItemConditions] PRIMARY KEY CLUSTERED 
(
	[ItemConditionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemSubCategories]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemSubCategories](
	[ItemSubCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[ItemTypeId] [int] NULL,
	[ItemSubCategoryName] [nvarchar](max) NOT NULL,
	[ItemSubCategoryDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.ItemSubCategories] PRIMARY KEY CLUSTERED 
(
	[ItemSubCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemTypes](
	[ItemTypeId] [int] IDENTITY(1,1) NOT NULL,
	[ItemTypeName] [nvarchar](max) NOT NULL,
	[ItemTypeDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[ItemTypeKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ItemTypes] PRIMARY KEY CLUSTERED 
(
	[ItemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LicenseApplicationConditions]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LicenseApplicationConditions](
	[LicenseApplicationConditionsId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[ConditionId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[ConditionName] [nvarchar](max) NULL,
	[IsSelected] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.LicenseApplicationConditions] PRIMARY KEY CLUSTERED 
(
	[LicenseApplicationConditionsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Licenses]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Licenses](
	[LicenseId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseTypeId] [int] NOT NULL,
	[ClientId] [int] NULL,
	[BusinessId] [int] NULL,
	[LicenseIssueYear] [int] NOT NULL,
	[ApplicationDateTime] [datetime] NULL,
	[LicenseIssueDateTime] [datetime] NULL,
	[NotificationUpdatedDateTime] [datetime] NULL,
	[StatusId] [int] NOT NULL,
	[LicenseClosureDateTime] [datetime] NULL,
	[LicenseCollectedDateTime] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[LicenseExpiryDate] [datetime] NULL,
	[LicenseNumber] [nvarchar](20) NULL,
	[ItemTypeId] [int] NULL,
	[ItemSubCategoryId] [int] NULL,
	[RegionId] [int] NULL,
	[ItemConditionId] [int] NOT NULL,
	[LicenseRenewalDateTime] [datetime] NULL,
	[ConditionReason] [nvarchar](max) NULL,
	[ConditionComment] [nvarchar](max) NULL,
	[migratedLicense] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Licenses] PRIMARY KEY CLUSTERED 
(
	[LicenseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LicensesRevokeds]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LicensesRevokeds](
	[LicensesRevokedId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[Decision] [nvarchar](50) NULL,
	[DecisionDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[PreviousStatusId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.LicensesRevokeds] PRIMARY KEY CLUSTERED 
(
	[LicensesRevokedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LicenseTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LicenseTypes](
	[LicenseTypeId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseTypeName] [nvarchar](100) NOT NULL,
	[LicenseTypeDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[LicenseTypeKey] [nvarchar](100) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.LicenseTypes] PRIMARY KEY CLUSTERED 
(
	[LicenseTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LookupTables]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LookupTables](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [nvarchar](200) NULL,
	[EscalationDay] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[RoleId] [nvarchar](128) NULL,
	[RoleName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.LookupTables] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ManagerReviews]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManagerReviews](
	[ManagerReviewId] [int] IDENTITY(1,1) NOT NULL,
	[Decision] [nvarchar](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[DateReviewed] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[UserId] [int] NOT NULL,
	[LicenseId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ManagerReviews] PRIMARY KEY CLUSTERED 
(
	[ManagerReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MigratedBusinesses]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MigratedBusinesses](
	[MBusinessId] [int] IDENTITY(1,1) NOT NULL,
	[MClientId] [int] NULL,
	[ProposedTradeName] [nvarchar](max) NULL,
	[KnownAs] [nvarchar](max) NULL,
	[SameAs] [bit] NULL,
	[BusinessTypeId] [int] NULL,
	[PostalAddress1] [nvarchar](max) NULL,
	[PostalAddress2] [nvarchar](max) NULL,
	[PostalAddress3] [nvarchar](max) NULL,
	[PostalAddressCode] [int] NULL,
	[ResidentialShopOrUnitNumber] [int] NULL,
	[ResidentialStreetAddress] [nvarchar](max) NULL,
	[ResidentialAddress1] [nvarchar](max) NULL,
	[ResidentialAddress2] [nvarchar](max) NULL,
	[ResidentialAddress3] [nvarchar](max) NULL,
	[ResidentialAddressCode] [int] NULL,
	[TelephoneNumber] [nvarchar](max) NULL,
	[CellphoneNumber] [nvarchar](max) NULL,
	[FaxNumber] [nvarchar](max) NULL,
	[TitleDeedTypeId] [int] NULL,
	[RatesAccountNumber] [nvarchar](100) NULL,
	[NameOfBusinessOperator] [nvarchar](max) NULL,
	[OperatorIdentityOrPassportNumber] [nvarchar](max) NULL,
	[OperatorResidentialAddress1] [nvarchar](max) NULL,
	[OperatorResidentialAddress2] [nvarchar](max) NULL,
	[OperatorResidentialAddress3] [nvarchar](max) NULL,
	[OperatorResidentialAddressCode] [int] NULL,
	[ItemTypeId] [int] NULL,
	[IsSelfEmployed] [bit] NULL,
	[NameOfEmployer] [nvarchar](max) NULL,
	[EmployerResidentialAddress1] [nvarchar](max) NULL,
	[EmployerResidentialAddress2] [nvarchar](max) NULL,
	[EmployerResidentialAddress3] [nvarchar](max) NULL,
	[EmployerResidentialAddressCode] [int] NULL,
	[OperationStructureTypeId] [int] NULL,
	[BusinessStatusId] [int] NULL,
	[BusinessOperatorId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[AltCellphoneNumber] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.MigratedBusinesses] PRIMARY KEY CLUSTERED 
(
	[MBusinessId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MigratedClients]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MigratedClients](
	[MClientId] [int] IDENTITY(1,1) NOT NULL,
	[IdentityOrPassportNumber] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Surname] [nvarchar](max) NULL,
	[Individual] [nvarchar](max) NULL,
	[CustomerType] [nvarchar](max) NULL,
	[RegNumber] [nvarchar](max) NULL,
	[BusinessName] [nvarchar](max) NULL,
	[SameAs] [bit] NULL,
	[Nationality] [nvarchar](max) NULL,
	[NationalityOther] [nvarchar](max) NULL,
	[ExpiryPermit] [nvarchar](max) NULL,
	[ResidentialAddress1] [nvarchar](max) NULL,
	[ResidentialAddress2] [nvarchar](max) NULL,
	[ResidentialAddress3] [nvarchar](max) NULL,
	[ResidentialAddressCode] [int] NULL,
	[PostalAddress1] [nvarchar](max) NULL,
	[PostalAddress2] [nvarchar](max) NULL,
	[PostalAddress3] [nvarchar](max) NULL,
	[PostalAddressCode] [int] NULL,
	[TelephoneNumber] [nvarchar](max) NULL,
	[CellphoneNumber] [nvarchar](max) NULL,
	[FaxNumber] [nvarchar](max) NULL,
	[EmailAddress] [nvarchar](max) NULL,
	[ClientStatusId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[AltCellphoneNumber] [nvarchar](100) NULL,
 CONSTRAINT [PK_dbo.MigratedClients] PRIMARY KEY CLUSTERED 
(
	[MClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MigratedLicenses]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MigratedLicenses](
	[MLicenseId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseTypeId] [int] NULL,
	[MClientId] [int] NULL,
	[ItemTypeId] [int] NULL,
	[ItemSubCategoryId] [int] NULL,
	[MBusinessId] [int] NULL,
	[LicenseNumber] [nvarchar](20) NULL,
	[RegionId] [int] NULL,
	[LicenseIssueYear] [int] NULL,
	[ApplicationDateTime] [datetime] NULL,
	[LicenseIssueDateTime] [datetime] NULL,
	[NotificationUpdatedDateTime] [datetime] NULL,
	[LicenseExpiryDate] [datetime] NULL,
	[StatusId] [int] NULL,
	[LicenseClosureDateTime] [datetime] NULL,
	[LicenseCollectedDateTime] [datetime] NULL,
	[LicenseRenewalDateTime] [datetime] NULL,
	[ItemConditionId] [int] NULL,
	[ConditionReason] [nvarchar](max) NULL,
	[ConditionComment] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.MigratedLicenses] PRIMARY KEY CLUSTERED 
(
	[MLicenseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NumOfDays]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NumOfDays](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[NumberOfDays] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.NumOfDays] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OperationStructureTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperationStructureTypes](
	[OperationStructureTypeId] [int] IDENTITY(1,1) NOT NULL,
	[OperationStructureTypeName] [nvarchar](max) NULL,
	[OperationStructureTypeDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.OperationStructureTypes] PRIMARY KEY CLUSTERED 
(
	[OperationStructureTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PaymentLicenses]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentLicenses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[CUST_ACCT_NO] [nvarchar](50) NULL,
	[SERVICE_UNIT] [nvarchar](50) NULL,
	[REQUEST_NO] [nvarchar](50) NULL,
	[PAYINSLIP_NO] [nvarchar](50) NULL,
	[PAYINSLIP_AMOUNT] [float] NOT NULL,
	[ALLOCATED_AMOUNT] [float] NOT NULL,
	[PAID_AMOUNT] [float] NOT NULL,
	[BALANCE_UNALLOCATED_AMOUNT] [float] NOT NULL,
	[PAYINSLIP_DATE] [nvarchar](10) NULL,
	[PAID_DATE] [nvarchar](10) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.PaymentLicenses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Payments]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentId] [int] IDENTITY(1,1) NOT NULL,
	[CUST_ACCT_NO] [nvarchar](50) NULL,
	[SERVICE_UNIT] [nvarchar](50) NULL,
	[REQUEST_NO] [nvarchar](50) NULL,
	[PAYINSLIP_NO] [nvarchar](50) NULL,
	[PAYINSLIP_AMOUNT] [float] NOT NULL,
	[ALLOCATED_AMOUNT] [float] NOT NULL,
	[PAID_AMOUNT] [float] NOT NULL,
	[BALANCE_UNALLOCATED_AMOUNT] [float] NOT NULL,
	[PAYINSLIP_DATE] [nvarchar](10) NULL,
	[PAID_DATE] [nvarchar](10) NULL,
 CONSTRAINT [PK_dbo.Payments] PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PublicHolidays]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublicHolidays](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [nvarchar](50) NULL,
	[Name] [nvarchar](150) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.PublicHolidays] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Refusals]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Refusals](
	[RefusalId] [int] IDENTITY(1,1) NOT NULL,
	[StatusId] [int] NOT NULL,
	[LicenseId] [int] NOT NULL,
	[InspectionResponseId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[DateLog] [datetime] NULL,
	[InspectionRequestId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Refusals] PRIMARY KEY CLUSTERED 
(
	[RefusalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Regions]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Regions](
	[RegionId] [int] IDENTITY(1,1) NOT NULL,
	[RegionName] [nvarchar](100) NOT NULL,
	[RegionKey] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.Regions] PRIMARY KEY CLUSTERED 
(
	[RegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RenewalHistories]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RenewalHistories](
	[RenewalHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[OldRenewalDateTime] [datetime] NULL,
	[NewRenewalDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.RenewalHistories] PRIMARY KEY CLUSTERED 
(
	[RenewalHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RevokeCancelReviews]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RevokeCancelReviews](
	[RevokeCancelReviewId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Decision] [nvarchar](50) NULL,
	[Comment] [nvarchar](max) NULL,
	[DateReviewed] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.RevokeCancelReviews] PRIMARY KEY CLUSTERED 
(
	[RevokeCancelReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceLevelAgreements]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceLevelAgreements](
	[SlaId] [int] IDENTITY(1,1) NOT NULL,
	[SlaDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[SlaKey] [nvarchar](max) NULL,
	[SlaName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ServiceLevelAgreements] PRIMARY KEY CLUSTERED 
(
	[SlaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Status]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [nvarchar](max) NOT NULL,
	[StatusTypeId] [int] NOT NULL,
	[StatusDescription] [nvarchar](max) NULL,
	[StatusKey] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.Status] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StatusTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusTypes](
	[StatusTypeId] [int] IDENTITY(1,1) NOT NULL,
	[StatusTypeName] [nvarchar](max) NULL,
	[StatusTypeDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.StatusTypes] PRIMARY KEY CLUSTERED 
(
	[StatusTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TitleDeedTypes]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TitleDeedTypes](
	[TitleDeedTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TitleDeedTypeName] [nvarchar](max) NULL,
	[TitleDeedDescription] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
 CONSTRAINT [PK_dbo.TitleDeedTypes] PRIMARY KEY CLUSTERED 
(
	[TitleDeedTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](max) NOT NULL,
	[LastName] [nvarchar](max) NOT NULL,
	[Username] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[EmailAddress] [nvarchar](max) NOT NULL,
	[LandLine] [nvarchar](max) NULL,
	[Mobile] [nvarchar](max) NULL,
	[Fax] [nvarchar](max) NULL,
	[IpAddress] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[IsPasswordReset] [bit] NULL,
	[Region] [int] NULL,
	[Role] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ViewSettings]    Script Date: 2/11/2021 1:07:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ViewSettings](
	[ViewSettingsId] [int] IDENTITY(1,1) NOT NULL,
	[ItemName] [nvarchar](max) NULL,
	[Role] [nvarchar](50) NULL,
	[Description] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsLocked] [bit] NOT NULL,
	[CreatedByUserId] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedByUserId] [int] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[ItemKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ViewSettings] PRIMARY KEY CLUSTERED 
(
	[ViewSettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Audits] ADD  DEFAULT ((0)) FOR [LicenseId]
GO
ALTER TABLE [dbo].[ChiefReviews] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ChiefReviews] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ChiefReviews] ADD  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[cmn_sequences] ADD  CONSTRAINT [cmn_sequences_insdate_default]  DEFAULT (getdate()) FOR [inserted_date]
GO
ALTER TABLE [dbo].[DocumentCheckLists] ADD  DEFAULT ((0)) FOR [DocumentId]
GO
ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
GO
ALTER TABLE [dbo].[Enquiries] ADD  DEFAULT ((0)) FOR [LicenseTypeId]
GO
ALTER TABLE [dbo].[Enquiries] ADD  DEFAULT ((0)) FOR [DepartmentId]
GO
ALTER TABLE [dbo].[FileUploads] ADD  DEFAULT ((0)) FOR [DocumentId]
GO
ALTER TABLE [dbo].[FileUploads] ADD  DEFAULT ((0)) FOR [referenceId]
GO
ALTER TABLE [dbo].[InspectionHistories] ADD  DEFAULT ((0)) FOR [CapturedByClerk]
GO
ALTER TABLE [dbo].[InspectionRequests] ADD  DEFAULT ((0)) FOR [DepartmentContactId]
GO
ALTER TABLE [dbo].[InspectionResponses] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [InspectionDateTime]
GO
ALTER TABLE [dbo].[InspectionResponses] ADD  DEFAULT ((0)) FOR [InspectionFailureCount]
GO
ALTER TABLE [dbo].[InspectionResponses] ADD  DEFAULT ((0)) FOR [AdhocInspector]
GO
ALTER TABLE [dbo].[InspectionResponses] ADD  DEFAULT ((0)) FOR [CapturedByClerk]
GO
ALTER TABLE [dbo].[ItemConditions] ADD  DEFAULT ('') FOR [ItemConditionName]
GO
ALTER TABLE [dbo].[LicenseApplicationConditions] ADD  DEFAULT ((0)) FOR [IsSelected]
GO
ALTER TABLE [dbo].[Licenses] ADD  DEFAULT ((0)) FOR [RegionId]
GO
ALTER TABLE [dbo].[Licenses] ADD  DEFAULT ((0)) FOR [ItemConditionId]
GO
ALTER TABLE [dbo].[LicensesRevokeds] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[LicensesRevokeds] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LicensesRevokeds] ADD  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[LicensesRevokeds] ADD  DEFAULT ((0)) FOR [PreviousStatusId]
GO
ALTER TABLE [dbo].[LicenseTypes] ADD  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[ManagerReviews] ADD  DEFAULT ((0)) FOR [UserId]
GO
ALTER TABLE [dbo].[ManagerReviews] ADD  DEFAULT ((0)) FOR [LicenseId]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT ((0)) FOR [PAYINSLIP_AMOUNT]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT ((0)) FOR [ALLOCATED_AMOUNT]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT ((0)) FOR [PAID_AMOUNT]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT ((0)) FOR [BALANCE_UNALLOCATED_AMOUNT]
GO
ALTER TABLE [dbo].[Refusals] ADD  DEFAULT ((0)) FOR [InspectionRequestId]
GO
ALTER TABLE [dbo].[AdministratorReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdministratorReviews_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[AdministratorReviews] CHECK CONSTRAINT [FK_dbo.AdministratorReviews_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[AdministratorReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdministratorReviews_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[AdministratorReviews] CHECK CONSTRAINT [FK_dbo.AdministratorReviews_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[AdministratorReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdministratorReviews_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[AdministratorReviews] CHECK CONSTRAINT [FK_dbo.AdministratorReviews_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[AdministratorReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdministratorReviews_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[AdministratorReviews] CHECK CONSTRAINT [FK_dbo.AdministratorReviews_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[Amendments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Amendments_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[Amendments] CHECK CONSTRAINT [FK_dbo.Amendments_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[Amendments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Amendments_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Amendments] CHECK CONSTRAINT [FK_dbo.Amendments_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Amendments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Amendments_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Amendments] CHECK CONSTRAINT [FK_dbo.Amendments_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUsers_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_dbo.AspNetUsers_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[BusinessEmployees]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessEmployees_dbo.Businesses_BusinessId] FOREIGN KEY([BusinessId])
REFERENCES [dbo].[Businesses] ([BusinessId])
GO
ALTER TABLE [dbo].[BusinessEmployees] CHECK CONSTRAINT [FK_dbo.BusinessEmployees_dbo.Businesses_BusinessId]
GO
ALTER TABLE [dbo].[BusinessEmployees]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessEmployees_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessEmployees] CHECK CONSTRAINT [FK_dbo.BusinessEmployees_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[BusinessEmployees]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessEmployees_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessEmployees] CHECK CONSTRAINT [FK_dbo.BusinessEmployees_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Businesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Businesses_dbo.BusinessTypes_BusinessTypeId] FOREIGN KEY([BusinessTypeId])
REFERENCES [dbo].[BusinessTypes] ([BusinessTypeId])
GO
ALTER TABLE [dbo].[Businesses] CHECK CONSTRAINT [FK_dbo.Businesses_dbo.BusinessTypes_BusinessTypeId]
GO
ALTER TABLE [dbo].[Businesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Businesses_dbo.Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Clients] ([ClientId])
GO
ALTER TABLE [dbo].[Businesses] CHECK CONSTRAINT [FK_dbo.Businesses_dbo.Clients_ClientId]
GO
ALTER TABLE [dbo].[Businesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Businesses_dbo.ItemTypes_ItemTypeId] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([ItemTypeId])
GO
ALTER TABLE [dbo].[Businesses] CHECK CONSTRAINT [FK_dbo.Businesses_dbo.ItemTypes_ItemTypeId]
GO
ALTER TABLE [dbo].[Businesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Businesses_dbo.OperationStructureTypes_OperationStructureTypeId] FOREIGN KEY([OperationStructureTypeId])
REFERENCES [dbo].[OperationStructureTypes] ([OperationStructureTypeId])
GO
ALTER TABLE [dbo].[Businesses] CHECK CONSTRAINT [FK_dbo.Businesses_dbo.OperationStructureTypes_OperationStructureTypeId]
GO
ALTER TABLE [dbo].[Businesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Businesses_dbo.Status_BusinessStatusId] FOREIGN KEY([BusinessStatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[Businesses] CHECK CONSTRAINT [FK_dbo.Businesses_dbo.Status_BusinessStatusId]
GO
ALTER TABLE [dbo].[Businesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Businesses_dbo.TitleDeedTypes_TitleDeedTypeId] FOREIGN KEY([TitleDeedTypeId])
REFERENCES [dbo].[TitleDeedTypes] ([TitleDeedTypeId])
GO
ALTER TABLE [dbo].[Businesses] CHECK CONSTRAINT [FK_dbo.Businesses_dbo.TitleDeedTypes_TitleDeedTypeId]
GO
ALTER TABLE [dbo].[Businesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Businesses_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Businesses] CHECK CONSTRAINT [FK_dbo.Businesses_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Businesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Businesses_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Businesses] CHECK CONSTRAINT [FK_dbo.Businesses_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[BusinessMangers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessMangers_dbo.Businesses_BusinessId] FOREIGN KEY([BusinessId])
REFERENCES [dbo].[Businesses] ([BusinessId])
GO
ALTER TABLE [dbo].[BusinessMangers] CHECK CONSTRAINT [FK_dbo.BusinessMangers_dbo.Businesses_BusinessId]
GO
ALTER TABLE [dbo].[BusinessMangers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessMangers_dbo.BusinessOperators_BusinessOperatorId] FOREIGN KEY([BusinessOperatorId])
REFERENCES [dbo].[BusinessOperators] ([BusinessOperatorId])
GO
ALTER TABLE [dbo].[BusinessMangers] CHECK CONSTRAINT [FK_dbo.BusinessMangers_dbo.BusinessOperators_BusinessOperatorId]
GO
ALTER TABLE [dbo].[BusinessMangers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessMangers_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessMangers] CHECK CONSTRAINT [FK_dbo.BusinessMangers_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[BusinessMangers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessMangers_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessMangers] CHECK CONSTRAINT [FK_dbo.BusinessMangers_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[BusinessOperationTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessOperationTypes_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessOperationTypes] CHECK CONSTRAINT [FK_dbo.BusinessOperationTypes_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[BusinessOperationTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessOperationTypes_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessOperationTypes] CHECK CONSTRAINT [FK_dbo.BusinessOperationTypes_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[BusinessOperators]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessOperators_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessOperators] CHECK CONSTRAINT [FK_dbo.BusinessOperators_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[BusinessOperators]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessOperators_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessOperators] CHECK CONSTRAINT [FK_dbo.BusinessOperators_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[BusinessTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessTypes_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessTypes] CHECK CONSTRAINT [FK_dbo.BusinessTypes_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[BusinessTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BusinessTypes_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[BusinessTypes] CHECK CONSTRAINT [FK_dbo.BusinessTypes_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[ChiefReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChiefReviews_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[ChiefReviews] CHECK CONSTRAINT [FK_dbo.ChiefReviews_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[ChiefReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChiefReviews_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ChiefReviews] CHECK CONSTRAINT [FK_dbo.ChiefReviews_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[ChiefReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChiefReviews_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ChiefReviews] CHECK CONSTRAINT [FK_dbo.ChiefReviews_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[ChiefReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChiefReviews_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ChiefReviews] CHECK CONSTRAINT [FK_dbo.ChiefReviews_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[Clients]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Clients_dbo.Status_ClientStatusId] FOREIGN KEY([ClientStatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[Clients] CHECK CONSTRAINT [FK_dbo.Clients_dbo.Status_ClientStatusId]
GO
ALTER TABLE [dbo].[Clients]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Clients_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Clients] CHECK CONSTRAINT [FK_dbo.Clients_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Clients]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Clients_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Clients] CHECK CONSTRAINT [FK_dbo.Clients_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Conditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Conditions_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Conditions] CHECK CONSTRAINT [FK_dbo.Conditions_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Conditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Conditions_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Conditions] CHECK CONSTRAINT [FK_dbo.Conditions_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[DepartmentContacts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentContacts_dbo.DepartmentContacts_DepartmentHeadContactId] FOREIGN KEY([DepartmentHeadContactId])
REFERENCES [dbo].[DepartmentContacts] ([DepartmentContactId])
GO
ALTER TABLE [dbo].[DepartmentContacts] CHECK CONSTRAINT [FK_dbo.DepartmentContacts_dbo.DepartmentContacts_DepartmentHeadContactId]
GO
ALTER TABLE [dbo].[DepartmentContacts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentContacts_dbo.Departments_DepartmentId] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
GO
ALTER TABLE [dbo].[DepartmentContacts] CHECK CONSTRAINT [FK_dbo.DepartmentContacts_dbo.Departments_DepartmentId]
GO
ALTER TABLE [dbo].[DepartmentContacts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentContacts_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DepartmentContacts] CHECK CONSTRAINT [FK_dbo.DepartmentContacts_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[DepartmentContacts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentContacts_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DepartmentContacts] CHECK CONSTRAINT [FK_dbo.DepartmentContacts_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[DepartmentContacts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentContacts_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DepartmentContacts] CHECK CONSTRAINT [FK_dbo.DepartmentContacts_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[Departments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Departments_dbo.Regions_RegionId] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Regions] ([RegionId])
GO
ALTER TABLE [dbo].[Departments] CHECK CONSTRAINT [FK_dbo.Departments_dbo.Regions_RegionId]
GO
ALTER TABLE [dbo].[Departments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Departments_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Departments] CHECK CONSTRAINT [FK_dbo.Departments_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Departments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Departments_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Departments] CHECK CONSTRAINT [FK_dbo.Departments_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[DepartmentServiceLevelAgreements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentServiceLevelAgreements_dbo.Departments_DepartmentId] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
GO
ALTER TABLE [dbo].[DepartmentServiceLevelAgreements] CHECK CONSTRAINT [FK_dbo.DepartmentServiceLevelAgreements_dbo.Departments_DepartmentId]
GO
ALTER TABLE [dbo].[DepartmentServiceLevelAgreements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentServiceLevelAgreements_dbo.ServiceLevelAgreements_SlaId] FOREIGN KEY([SlaId])
REFERENCES [dbo].[ServiceLevelAgreements] ([SlaId])
GO
ALTER TABLE [dbo].[DepartmentServiceLevelAgreements] CHECK CONSTRAINT [FK_dbo.DepartmentServiceLevelAgreements_dbo.ServiceLevelAgreements_SlaId]
GO
ALTER TABLE [dbo].[DepartmentServiceLevelAgreements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentServiceLevelAgreements_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DepartmentServiceLevelAgreements] CHECK CONSTRAINT [FK_dbo.DepartmentServiceLevelAgreements_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[DepartmentServiceLevelAgreements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DepartmentServiceLevelAgreements_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DepartmentServiceLevelAgreements] CHECK CONSTRAINT [FK_dbo.DepartmentServiceLevelAgreements_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[DocumentCheckLists]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DocumentCheckLists_dbo.Documents_DocumentId] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Documents] ([DocumentId])
GO
ALTER TABLE [dbo].[DocumentCheckLists] CHECK CONSTRAINT [FK_dbo.DocumentCheckLists_dbo.Documents_DocumentId]
GO
ALTER TABLE [dbo].[DocumentCheckLists]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DocumentCheckLists_dbo.LicenseTypes_LicenseTypeId] FOREIGN KEY([LicenseTypeId])
REFERENCES [dbo].[LicenseTypes] ([LicenseTypeId])
GO
ALTER TABLE [dbo].[DocumentCheckLists] CHECK CONSTRAINT [FK_dbo.DocumentCheckLists_dbo.LicenseTypes_LicenseTypeId]
GO
ALTER TABLE [dbo].[DocumentCheckLists]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DocumentCheckLists_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DocumentCheckLists] CHECK CONSTRAINT [FK_dbo.DocumentCheckLists_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[DocumentCheckLists]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DocumentCheckLists_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DocumentCheckLists] CHECK CONSTRAINT [FK_dbo.DocumentCheckLists_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Documents_dbo.DocumentTypes_DocumentTypeId] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentTypes] ([DocumentTypeId])
GO
ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [FK_dbo.Documents_dbo.DocumentTypes_DocumentTypeId]
GO
ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Documents_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [FK_dbo.Documents_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Documents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Documents_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Documents] CHECK CONSTRAINT [FK_dbo.Documents_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[DocumentTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DocumentTypes_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DocumentTypes] CHECK CONSTRAINT [FK_dbo.DocumentTypes_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[DocumentTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DocumentTypes_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DocumentTypes] CHECK CONSTRAINT [FK_dbo.DocumentTypes_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[DropdownItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DropdownItems_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DropdownItems] CHECK CONSTRAINT [FK_dbo.DropdownItems_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[DropdownItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DropdownItems_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[DropdownItems] CHECK CONSTRAINT [FK_dbo.DropdownItems_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Enquiries]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Enquiries_dbo.Departments_DepartmentId] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
GO
ALTER TABLE [dbo].[Enquiries] CHECK CONSTRAINT [FK_dbo.Enquiries_dbo.Departments_DepartmentId]
GO
ALTER TABLE [dbo].[Enquiries]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Enquiries_dbo.LicenseTypes_LicenseTypeId] FOREIGN KEY([LicenseTypeId])
REFERENCES [dbo].[LicenseTypes] ([LicenseTypeId])
GO
ALTER TABLE [dbo].[Enquiries] CHECK CONSTRAINT [FK_dbo.Enquiries_dbo.LicenseTypes_LicenseTypeId]
GO
ALTER TABLE [dbo].[Enquiries]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Enquiries_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Enquiries] CHECK CONSTRAINT [FK_dbo.Enquiries_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Enquiries]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Enquiries_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Enquiries] CHECK CONSTRAINT [FK_dbo.Enquiries_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[EscalationMainTables]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EscalationMainTables_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[EscalationMainTables] CHECK CONSTRAINT [FK_dbo.EscalationMainTables_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[EscalationMainTables]  WITH CHECK ADD  CONSTRAINT [FK_dbo.EscalationMainTables_dbo.Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[EscalationMainTables] CHECK CONSTRAINT [FK_dbo.EscalationMainTables_dbo.Status_StatusId]
GO
ALTER TABLE [dbo].[FileUploads]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FileUploads_dbo.Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Clients] ([ClientId])
GO
ALTER TABLE [dbo].[FileUploads] CHECK CONSTRAINT [FK_dbo.FileUploads_dbo.Clients_ClientId]
GO
ALTER TABLE [dbo].[FileUploads]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FileUploads_dbo.Documents_DocumentId] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Documents] ([DocumentId])
GO
ALTER TABLE [dbo].[FileUploads] CHECK CONSTRAINT [FK_dbo.FileUploads_dbo.Documents_DocumentId]
GO
ALTER TABLE [dbo].[FileUploads]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FileUploads_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[FileUploads] CHECK CONSTRAINT [FK_dbo.FileUploads_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[FileUploads]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FileUploads_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[FileUploads] CHECK CONSTRAINT [FK_dbo.FileUploads_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[InspectionAppeals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionAppeals_dbo.InspectionResponses_InspectionResponseId] FOREIGN KEY([InspectionResponseId])
REFERENCES [dbo].[InspectionResponses] ([InspectionResponseId])
GO
ALTER TABLE [dbo].[InspectionAppeals] CHECK CONSTRAINT [FK_dbo.InspectionAppeals_dbo.InspectionResponses_InspectionResponseId]
GO
ALTER TABLE [dbo].[InspectionAppeals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionAppeals_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[InspectionAppeals] CHECK CONSTRAINT [FK_dbo.InspectionAppeals_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[InspectionAppeals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionAppeals_dbo.Status_Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[InspectionAppeals] CHECK CONSTRAINT [FK_dbo.InspectionAppeals_dbo.Status_Status_StatusId]
GO
ALTER TABLE [dbo].[InspectionAppealsReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionAppealsReviews_dbo.InspectionAppeals_InspectionAppealId] FOREIGN KEY([InspectionAppealId])
REFERENCES [dbo].[InspectionAppeals] ([InspectionAppealId])
GO
ALTER TABLE [dbo].[InspectionAppealsReviews] CHECK CONSTRAINT [FK_dbo.InspectionAppealsReviews_dbo.InspectionAppeals_InspectionAppealId]
GO
ALTER TABLE [dbo].[InspectionAppealsReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionAppealsReviews_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[InspectionAppealsReviews] CHECK CONSTRAINT [FK_dbo.InspectionAppealsReviews_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[InspectionAppealsReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionAppealsReviews_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[InspectionAppealsReviews] CHECK CONSTRAINT [FK_dbo.InspectionAppealsReviews_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[InspectionAppealsReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionAppealsReviews_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[InspectionAppealsReviews] CHECK CONSTRAINT [FK_dbo.InspectionAppealsReviews_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[InspectionHistories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionHistories_dbo.InspectionRequests_InspectionRequestId] FOREIGN KEY([InspectionRequestId])
REFERENCES [dbo].[InspectionRequests] ([InspectionRequestId])
GO
ALTER TABLE [dbo].[InspectionHistories] CHECK CONSTRAINT [FK_dbo.InspectionHistories_dbo.InspectionRequests_InspectionRequestId]
GO
ALTER TABLE [dbo].[InspectionHistories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionHistories_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[InspectionHistories] CHECK CONSTRAINT [FK_dbo.InspectionHistories_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[InspectionHistories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionHistories_dbo.Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[InspectionHistories] CHECK CONSTRAINT [FK_dbo.InspectionHistories_dbo.Status_StatusId]
GO
ALTER TABLE [dbo].[InspectionRequests]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionRequests_dbo.Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Clients] ([ClientId])
GO
ALTER TABLE [dbo].[InspectionRequests] CHECK CONSTRAINT [FK_dbo.InspectionRequests_dbo.Clients_ClientId]
GO
ALTER TABLE [dbo].[InspectionRequests]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionRequests_dbo.Departments_DepartmentId] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
GO
ALTER TABLE [dbo].[InspectionRequests] CHECK CONSTRAINT [FK_dbo.InspectionRequests_dbo.Departments_DepartmentId]
GO
ALTER TABLE [dbo].[InspectionRequests]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionRequests_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[InspectionRequests] CHECK CONSTRAINT [FK_dbo.InspectionRequests_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[InspectionRequests]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionRequests_dbo.LicenseTypes_LicenseTypeId] FOREIGN KEY([LicenseTypeId])
REFERENCES [dbo].[LicenseTypes] ([LicenseTypeId])
GO
ALTER TABLE [dbo].[InspectionRequests] CHECK CONSTRAINT [FK_dbo.InspectionRequests_dbo.LicenseTypes_LicenseTypeId]
GO
ALTER TABLE [dbo].[InspectionRequests]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionRequests_dbo.Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[InspectionRequests] CHECK CONSTRAINT [FK_dbo.InspectionRequests_dbo.Status_StatusId]
GO
ALTER TABLE [dbo].[InspectionRequests]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionRequests_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[InspectionRequests] CHECK CONSTRAINT [FK_dbo.InspectionRequests_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[InspectionRequests]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionRequests_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[InspectionRequests] CHECK CONSTRAINT [FK_dbo.InspectionRequests_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[InspectionResponses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionResponses_dbo.InspectionRequests_InspectionRequestId] FOREIGN KEY([InspectionRequestId])
REFERENCES [dbo].[InspectionRequests] ([InspectionRequestId])
GO
ALTER TABLE [dbo].[InspectionResponses] CHECK CONSTRAINT [FK_dbo.InspectionResponses_dbo.InspectionRequests_InspectionRequestId]
GO
ALTER TABLE [dbo].[InspectionResponses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionResponses_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[InspectionResponses] CHECK CONSTRAINT [FK_dbo.InspectionResponses_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[InspectionResponses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionResponses_dbo.Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[InspectionResponses] CHECK CONSTRAINT [FK_dbo.InspectionResponses_dbo.Status_StatusId]
GO
ALTER TABLE [dbo].[InspectionResponses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionResponses_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[InspectionResponses] CHECK CONSTRAINT [FK_dbo.InspectionResponses_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[InspectionResponses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InspectionResponses_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[InspectionResponses] CHECK CONSTRAINT [FK_dbo.InspectionResponses_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[ItemConditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemConditions_dbo.ItemTypes_ItemTypeId] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([ItemTypeId])
GO
ALTER TABLE [dbo].[ItemConditions] CHECK CONSTRAINT [FK_dbo.ItemConditions_dbo.ItemTypes_ItemTypeId]
GO
ALTER TABLE [dbo].[ItemConditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemConditions_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ItemConditions] CHECK CONSTRAINT [FK_dbo.ItemConditions_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[ItemConditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemConditions_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ItemConditions] CHECK CONSTRAINT [FK_dbo.ItemConditions_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[ItemSubCategories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemSubCategories_dbo.ItemTypes_ItemTypeId] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([ItemTypeId])
GO
ALTER TABLE [dbo].[ItemSubCategories] CHECK CONSTRAINT [FK_dbo.ItemSubCategories_dbo.ItemTypes_ItemTypeId]
GO
ALTER TABLE [dbo].[ItemSubCategories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemSubCategories_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ItemSubCategories] CHECK CONSTRAINT [FK_dbo.ItemSubCategories_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[ItemSubCategories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemSubCategories_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ItemSubCategories] CHECK CONSTRAINT [FK_dbo.ItemSubCategories_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[ItemTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemTypes_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ItemTypes] CHECK CONSTRAINT [FK_dbo.ItemTypes_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[ItemTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ItemTypes_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ItemTypes] CHECK CONSTRAINT [FK_dbo.ItemTypes_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[LicenseApplicationConditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicenseApplicationConditions_dbo.Conditions_ConditionId] FOREIGN KEY([ConditionId])
REFERENCES [dbo].[Conditions] ([ConditionId])
GO
ALTER TABLE [dbo].[LicenseApplicationConditions] CHECK CONSTRAINT [FK_dbo.LicenseApplicationConditions_dbo.Conditions_ConditionId]
GO
ALTER TABLE [dbo].[LicenseApplicationConditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicenseApplicationConditions_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[LicenseApplicationConditions] CHECK CONSTRAINT [FK_dbo.LicenseApplicationConditions_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[LicenseApplicationConditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicenseApplicationConditions_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[LicenseApplicationConditions] CHECK CONSTRAINT [FK_dbo.LicenseApplicationConditions_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[LicenseApplicationConditions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicenseApplicationConditions_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[LicenseApplicationConditions] CHECK CONSTRAINT [FK_dbo.LicenseApplicationConditions_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.Businesses_BusinessId] FOREIGN KEY([BusinessId])
REFERENCES [dbo].[Businesses] ([BusinessId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.Businesses_BusinessId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Clients] ([ClientId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.Clients_ClientId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.ItemConditions_ItemConditionId] FOREIGN KEY([ItemConditionId])
REFERENCES [dbo].[ItemConditions] ([ItemConditionId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.ItemConditions_ItemConditionId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.ItemSubCategories_ItemSubCategoryId] FOREIGN KEY([ItemSubCategoryId])
REFERENCES [dbo].[ItemSubCategories] ([ItemSubCategoryId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.ItemSubCategories_ItemSubCategoryId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.ItemTypes_ItemTypeId] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([ItemTypeId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.ItemTypes_ItemTypeId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.LicenseTypes_LicenseTypeId] FOREIGN KEY([LicenseTypeId])
REFERENCES [dbo].[LicenseTypes] ([LicenseTypeId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.LicenseTypes_LicenseTypeId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.Regions_RegionId] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Regions] ([RegionId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.Regions_RegionId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.Status_StatusId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Licenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Licenses_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Licenses] CHECK CONSTRAINT [FK_dbo.Licenses_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[LicensesRevokeds]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicensesRevokeds_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[LicensesRevokeds] CHECK CONSTRAINT [FK_dbo.LicensesRevokeds_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[LicensesRevokeds]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicensesRevokeds_dbo.Status_PreviousStatusId] FOREIGN KEY([PreviousStatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[LicensesRevokeds] CHECK CONSTRAINT [FK_dbo.LicensesRevokeds_dbo.Status_PreviousStatusId]
GO
ALTER TABLE [dbo].[LicensesRevokeds]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicensesRevokeds_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[LicensesRevokeds] CHECK CONSTRAINT [FK_dbo.LicensesRevokeds_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[LicensesRevokeds]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicensesRevokeds_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[LicensesRevokeds] CHECK CONSTRAINT [FK_dbo.LicensesRevokeds_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[LicenseTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicenseTypes_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[LicenseTypes] CHECK CONSTRAINT [FK_dbo.LicenseTypes_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[LicenseTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LicenseTypes_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[LicenseTypes] CHECK CONSTRAINT [FK_dbo.LicenseTypes_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[LookupTables]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LookupTables_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
GO
ALTER TABLE [dbo].[LookupTables] CHECK CONSTRAINT [FK_dbo.LookupTables_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[LookupTables]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LookupTables_dbo.Status_StatusID] FOREIGN KEY([StatusID])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[LookupTables] CHECK CONSTRAINT [FK_dbo.LookupTables_dbo.Status_StatusID]
GO
ALTER TABLE [dbo].[ManagerReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ManagerReviews_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[ManagerReviews] CHECK CONSTRAINT [FK_dbo.ManagerReviews_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[ManagerReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ManagerReviews_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ManagerReviews] CHECK CONSTRAINT [FK_dbo.ManagerReviews_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[ManagerReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ManagerReviews_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ManagerReviews] CHECK CONSTRAINT [FK_dbo.ManagerReviews_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[ManagerReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ManagerReviews_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ManagerReviews] CHECK CONSTRAINT [FK_dbo.ManagerReviews_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.BusinessOperators_BusinessOperatorId] FOREIGN KEY([BusinessOperatorId])
REFERENCES [dbo].[BusinessOperators] ([BusinessOperatorId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.BusinessOperators_BusinessOperatorId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.BusinessTypes_BusinessTypeId] FOREIGN KEY([BusinessTypeId])
REFERENCES [dbo].[BusinessTypes] ([BusinessTypeId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.BusinessTypes_BusinessTypeId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.ItemTypes_ItemTypeId] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([ItemTypeId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.ItemTypes_ItemTypeId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.MigratedClients_MClientId] FOREIGN KEY([MClientId])
REFERENCES [dbo].[MigratedClients] ([MClientId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.MigratedClients_MClientId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.OperationStructureTypes_OperationStructureTypeId] FOREIGN KEY([OperationStructureTypeId])
REFERENCES [dbo].[OperationStructureTypes] ([OperationStructureTypeId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.OperationStructureTypes_OperationStructureTypeId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.Status_BusinessStatusId] FOREIGN KEY([BusinessStatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.Status_BusinessStatusId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.TitleDeedTypes_TitleDeedTypeId] FOREIGN KEY([TitleDeedTypeId])
REFERENCES [dbo].[TitleDeedTypes] ([TitleDeedTypeId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.TitleDeedTypes_TitleDeedTypeId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[MigratedBusinesses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[MigratedBusinesses] CHECK CONSTRAINT [FK_dbo.MigratedBusinesses_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[MigratedClients]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedClients_dbo.Status_ClientStatusId] FOREIGN KEY([ClientStatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[MigratedClients] CHECK CONSTRAINT [FK_dbo.MigratedClients_dbo.Status_ClientStatusId]
GO
ALTER TABLE [dbo].[MigratedClients]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedClients_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[MigratedClients] CHECK CONSTRAINT [FK_dbo.MigratedClients_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[MigratedClients]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedClients_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[MigratedClients] CHECK CONSTRAINT [FK_dbo.MigratedClients_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.ItemConditions_ItemConditionId] FOREIGN KEY([ItemConditionId])
REFERENCES [dbo].[ItemConditions] ([ItemConditionId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.ItemConditions_ItemConditionId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.ItemSubCategories_ItemSubCategoryId] FOREIGN KEY([ItemSubCategoryId])
REFERENCES [dbo].[ItemSubCategories] ([ItemSubCategoryId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.ItemSubCategories_ItemSubCategoryId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.ItemTypes_ItemTypeId] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([ItemTypeId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.ItemTypes_ItemTypeId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.LicenseTypes_LicenseTypeId] FOREIGN KEY([LicenseTypeId])
REFERENCES [dbo].[LicenseTypes] ([LicenseTypeId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.LicenseTypes_LicenseTypeId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.MigratedBusinesses_MBusinessId] FOREIGN KEY([MBusinessId])
REFERENCES [dbo].[MigratedBusinesses] ([MBusinessId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.MigratedBusinesses_MBusinessId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.MigratedClients_MClientId] FOREIGN KEY([MClientId])
REFERENCES [dbo].[MigratedClients] ([MClientId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.MigratedClients_MClientId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.Regions_RegionId] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Regions] ([RegionId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.Regions_RegionId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.Status_StatusId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[MigratedLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MigratedLicenses_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[MigratedLicenses] CHECK CONSTRAINT [FK_dbo.MigratedLicenses_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[NumOfDays]  WITH CHECK ADD  CONSTRAINT [FK_dbo.NumOfDays_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[NumOfDays] CHECK CONSTRAINT [FK_dbo.NumOfDays_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[NumOfDays]  WITH CHECK ADD  CONSTRAINT [FK_dbo.NumOfDays_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[NumOfDays] CHECK CONSTRAINT [FK_dbo.NumOfDays_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[OperationStructureTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OperationStructureTypes_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[OperationStructureTypes] CHECK CONSTRAINT [FK_dbo.OperationStructureTypes_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[OperationStructureTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OperationStructureTypes_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[OperationStructureTypes] CHECK CONSTRAINT [FK_dbo.OperationStructureTypes_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[PaymentLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PaymentLicenses_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[PaymentLicenses] CHECK CONSTRAINT [FK_dbo.PaymentLicenses_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[PaymentLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PaymentLicenses_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[PaymentLicenses] CHECK CONSTRAINT [FK_dbo.PaymentLicenses_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[PaymentLicenses]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PaymentLicenses_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[PaymentLicenses] CHECK CONSTRAINT [FK_dbo.PaymentLicenses_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[PublicHolidays]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PublicHolidays_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[PublicHolidays] CHECK CONSTRAINT [FK_dbo.PublicHolidays_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[PublicHolidays]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PublicHolidays_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[PublicHolidays] CHECK CONSTRAINT [FK_dbo.PublicHolidays_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Refusals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Refusals_dbo.InspectionRequests_InspectionRequestId] FOREIGN KEY([InspectionRequestId])
REFERENCES [dbo].[InspectionRequests] ([InspectionRequestId])
GO
ALTER TABLE [dbo].[Refusals] CHECK CONSTRAINT [FK_dbo.Refusals_dbo.InspectionRequests_InspectionRequestId]
GO
ALTER TABLE [dbo].[Refusals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Refusals_dbo.InspectionResponses_InspectionResponseId] FOREIGN KEY([InspectionResponseId])
REFERENCES [dbo].[InspectionResponses] ([InspectionResponseId])
GO
ALTER TABLE [dbo].[Refusals] CHECK CONSTRAINT [FK_dbo.Refusals_dbo.InspectionResponses_InspectionResponseId]
GO
ALTER TABLE [dbo].[Refusals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Refusals_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[Refusals] CHECK CONSTRAINT [FK_dbo.Refusals_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[Refusals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Refusals_dbo.Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([StatusId])
GO
ALTER TABLE [dbo].[Refusals] CHECK CONSTRAINT [FK_dbo.Refusals_dbo.Status_StatusId]
GO
ALTER TABLE [dbo].[Refusals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Refusals_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Refusals] CHECK CONSTRAINT [FK_dbo.Refusals_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Refusals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Refusals_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Refusals] CHECK CONSTRAINT [FK_dbo.Refusals_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Regions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Regions_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Regions] CHECK CONSTRAINT [FK_dbo.Regions_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Regions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Regions_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Regions] CHECK CONSTRAINT [FK_dbo.Regions_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[RenewalHistories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RenewalHistories_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[RenewalHistories] CHECK CONSTRAINT [FK_dbo.RenewalHistories_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[RenewalHistories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RenewalHistories_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[RenewalHistories] CHECK CONSTRAINT [FK_dbo.RenewalHistories_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[RenewalHistories]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RenewalHistories_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[RenewalHistories] CHECK CONSTRAINT [FK_dbo.RenewalHistories_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[RevokeCancelReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RevokeCancelReviews_dbo.Licenses_LicenseId] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[Licenses] ([LicenseId])
GO
ALTER TABLE [dbo].[RevokeCancelReviews] CHECK CONSTRAINT [FK_dbo.RevokeCancelReviews_dbo.Licenses_LicenseId]
GO
ALTER TABLE [dbo].[RevokeCancelReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RevokeCancelReviews_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[RevokeCancelReviews] CHECK CONSTRAINT [FK_dbo.RevokeCancelReviews_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[RevokeCancelReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RevokeCancelReviews_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[RevokeCancelReviews] CHECK CONSTRAINT [FK_dbo.RevokeCancelReviews_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[RevokeCancelReviews]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RevokeCancelReviews_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[RevokeCancelReviews] CHECK CONSTRAINT [FK_dbo.RevokeCancelReviews_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[ServiceLevelAgreements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ServiceLevelAgreements_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ServiceLevelAgreements] CHECK CONSTRAINT [FK_dbo.ServiceLevelAgreements_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[ServiceLevelAgreements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ServiceLevelAgreements_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ServiceLevelAgreements] CHECK CONSTRAINT [FK_dbo.ServiceLevelAgreements_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Status]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Status_dbo.StatusTypes_StatusTypeId] FOREIGN KEY([StatusTypeId])
REFERENCES [dbo].[StatusTypes] ([StatusTypeId])
GO
ALTER TABLE [dbo].[Status] CHECK CONSTRAINT [FK_dbo.Status_dbo.StatusTypes_StatusTypeId]
GO
ALTER TABLE [dbo].[Status]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Status_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Status] CHECK CONSTRAINT [FK_dbo.Status_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Status]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Status_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Status] CHECK CONSTRAINT [FK_dbo.Status_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[StatusTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.StatusTypes_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[StatusTypes] CHECK CONSTRAINT [FK_dbo.StatusTypes_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[StatusTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.StatusTypes_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[StatusTypes] CHECK CONSTRAINT [FK_dbo.StatusTypes_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[TitleDeedTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TitleDeedTypes_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[TitleDeedTypes] CHECK CONSTRAINT [FK_dbo.TitleDeedTypes_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[TitleDeedTypes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TitleDeedTypes_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[TitleDeedTypes] CHECK CONSTRAINT [FK_dbo.TitleDeedTypes_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Users_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_dbo.Users_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Users_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_dbo.Users_dbo.Users_ModifiedByUserId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Region] FOREIGN KEY([Region])
REFERENCES [dbo].[Regions] ([RegionId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Region]
GO
ALTER TABLE [dbo].[ViewSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ViewSettings_dbo.Users_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ViewSettings] CHECK CONSTRAINT [FK_dbo.ViewSettings_dbo.Users_CreatedByUserId]
GO
ALTER TABLE [dbo].[ViewSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ViewSettings_dbo.Users_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ViewSettings] CHECK CONSTRAINT [FK_dbo.ViewSettings_dbo.Users_ModifiedByUserId]
GO
/****** Object:  StoredProcedure [dbo].[Abandoned_sp]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--run via job everyday at midnigth
CREATE PROCEDURE [dbo].[Abandoned_sp]
--@case_desc datetime output
as
begin


	
	declare @current datetime,
			@InspectionFailed int,
			@RefusalInProgress int,
			@statusIdAbandon int,
			@statusIdAppeal int

set @current =  CAST(getdate()   AS DATE)


select @InspectionFailed =[StatusId] from [dbo].[Status] where [StatusKey] ='InspectionFailed'
select @statusIdAbandon = [StatusId] from [dbo].[Status] where [StatusKey] ='AppealAbandoned'
select @RefusalInProgress= [StatusId] from [dbo].[Status] where [StatusKey] ='RefusalInProgress'
select @statusIdAppeal= [StatusId] from [dbo].[Status] where [StatusKey] ='AppealApproved'

 UPDATE License
 SET License.[StatusId]= @statusIdAbandon
 from [dbo].[Licenses] as License
INNER JOIN [dbo].[InspectionRequests] as InspectionRequests ON License.[LicenseId]= InspectionRequests.[LicenseId]
 where  CAST(InspectionRequests.[AppealFinalDate] AS DATE)= @current And InspectionRequests.StatusId = @InspectionFailed


  UPDATE License
 SET License.[StatusId]= @statusIdAbandon
 from [dbo].[Licenses] as License
INNER JOIN [dbo].[InspectionAppeals]as appeals ON License.[LicenseId]= appeals.[LicenseId] 
 where  CAST(appeals.[AppealDateTime] AS DATE)= @current And appeals.StatusId = @statusIdAppeal

	

end

GO
/****** Object:  StoredProcedure [dbo].[ChangeStatus_sp]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ChangeStatus_sp]
@case_desc datetime output
as
begin


	
	declare @current datetime,
			@thirtyday datetime,
			@statusIdRenew int,
			@statusIdAbandon int

set @current =  CAST(getdate()   AS DATE)
set @thirtyday = CAST(dateadd(day,30,@current )   AS DATE)

select @statusIdRenew = [StatusId] from [dbo].[Status] where [StatusKey] ='LicenseRenewal'
select @statusIdAbandon = [StatusId] from [dbo].[Status] where [StatusKey] ='AppealAbandoned'

UPDATE [dbo].[Licenses]
   SET [StatusId] = @statusIdRenew 
where  CAST([LicenseExpiryDate] AS DATE)= @thirtyday

 UPDATE License
 SET License.[StatusId]= @statusIdAbandon
 from [dbo].[Licenses] as License
INNER JOIN [dbo].[InspectionAppeals] as Appeals ON License.[LicenseId]= Appeals.[LicenseId]
 where  CAST(Appeals.[AppealDateTime] AS DATE)= @current

UPDATE [dbo].[InspectionAppeals]
 SET [StatusId] = @statusIdAbandon
 where  CAST([AppealDateTime] AS DATE)= @current
	

end

GO
/****** Object:  StoredProcedure [dbo].[cmn_casecounter_sp]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[cmn_casecounter_sp]
@org_process_id int,
@seq_name varchar(10), -- prefix for city fleet (4) only
@case_desc varchar(24) output
as
begin
	set nocount on

	
	declare @icounter bigint,
			@imonth int,
			@imonth_tmp varchar(2),
			@iyear int,
			@iday int,
			@currmonth int,
			@curryear int,
			@currday int,
			@counter_value bigint,
			@counter_length int,
			@counter_charvalue varchar(20), 
			@prefix varchar(2),
			@proc_id int

	select  @prefix = @seq_name

	if @org_process_id = 4
		select @seq_name = 'VMC'

	select	@icounter = last_value, 
			@imonth = month,
			@iyear = year 
	from	[dbo].[cmn_sequences] with (updlock, rowlock)
	where   [org_process_id]	 = @org_process_id
	and		[seq_name] = @seq_name

	set @currmonth = month(getdate())
	set @curryear = year(getdate())
	set	@currday = day(getdate()) 

	if (@imonth = @currmonth and @iyear = @curryear)
	begin
		--select @icounter
		set @icounter = @icounter + 1
	end
	else
	begin
		set @icounter = 1
		set @imonth = @currmonth
		set @iyear = @curryear
		set @iday = @currday
	end

	set @counter_value = @icounter


	update	cmn_sequences 
	set		last_value = @counter_value, 
			month = @imonth, 
			year = @iyear 
	where	org_process_id = @org_process_id
	and		seq_name = @seq_name

	select	@case_desc = @seq_name 
	 

	if @imonth < 10
	begin 
		
		select @imonth_tmp = '0'+ convert(varchar(1),@imonth)


	end
	else
	begin
		select @imonth_tmp = @imonth
	end

	select @counter_length = len(@counter_value)
	select @counter_charvalue = convert(varchar(5), @counter_value)

	if @seq_name = 'BO' ---
	begin
	
		while @counter_length < 4
		begin
			select @counter_charvalue = stuff(@counter_charvalue, 1,0, '0')
			select @counter_length = @counter_length + 1
		end
		
		select  convert(varchar(4),@iyear) + @imonth_tmp + convert(varchar(4),@currday) + @counter_charvalue +'/' + @case_desc
		
		end

		else if @seq_name = 'BLS'
	begin
	
		while @counter_length < 4
		begin
			select @counter_charvalue = stuff(@counter_charvalue, 1,0, '0')
			select @counter_length = @counter_length + 1
		end
		
		select  convert(varchar(4),@iyear) + @imonth_tmp + convert(varchar(4),@currday) + @counter_charvalue +'/' + @case_desc
		
		end

		else if @seq_name = 'BN'
	begin
	
		while @counter_length < 4
		begin
			select @counter_charvalue = stuff(@counter_charvalue, 1,0, '0')
			select @counter_length = @counter_length + 1
		end
	
		select  convert(varchar(4),@iyear) + @imonth_tmp + convert(varchar(4),@currday) + @counter_charvalue +'/' + @case_desc
		
		end
			else if @seq_name = 'BW'
	begin
	
		while @counter_length < 4
		begin
			select @counter_charvalue = stuff(@counter_charvalue, 1,0, '0')
			select @counter_length = @counter_length + 1
		end
	
		select  convert(varchar(4),@iyear) + @imonth_tmp + convert(varchar(4),@currday) + @counter_charvalue +'/' + @case_desc
		
		end
	else if @seq_name = 'BLW'
	begin
	
		while @counter_length < 4
		begin
			select @counter_charvalue = stuff(@counter_charvalue, 1,0, '0')
			select @counter_length = @counter_length + 1
		end
		
		select  convert(varchar(4),@iyear) + @imonth_tmp + convert(varchar(4),@currday) + @counter_charvalue +'/' + @case_desc
		
		end
		else if @seq_name = 'IW'
	begin
	
		while @counter_length < 4
		begin
			select @counter_charvalue = stuff(@counter_charvalue, 1,0, '0')
			select @counter_length = @counter_length + 1
		end
			
		select  convert(varchar(4),@iyear) + @imonth_tmp + convert(varchar(4),@currday) + @counter_charvalue +'/' + @case_desc
		
		end

		else if @seq_name = 'OW'
	begin
	
		while @counter_length < 4
		begin
			select @counter_charvalue = stuff(@counter_charvalue, 1,0, '0')
			select @counter_length = @counter_length + 1
		end
	
		select  convert(varchar(4),@iyear) + @imonth_tmp + convert(varchar(4),@currday) + @counter_charvalue +'/' + @case_desc
		
		end




	else
	begin
		select  @case_desc = @case_desc + substring(convert(varchar(4),@iyear),3,2) + @imonth_tmp

		while @counter_length < 7
		begin
			select @counter_charvalue = stuff(@counter_charvalue, 1,0, '0')
			select @counter_length = @counter_length + 1
		end
	end
	
	select  @case_desc = @case_desc


end

GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]
(
    @Application NVARCHAR(60),
    @PageIndex INT = 0,
    @PageSize INT = 15,
    @TotalCount INT OUTPUT
)
AS 

    SET NOCOUNT ON

    DECLARE @FirstTimeUTC DATETIME
    DECLARE @FirstSequence INT
    DECLARE @StartRow INT
    DECLARE @StartRowIndex INT

    SELECT 
        @TotalCount = COUNT(1) 
    FROM 
        [ELMAH_Error]
    WHERE 
        [Application] = @Application

    -- Get the ID of the first error for the requested page

    SET @StartRowIndex = @PageIndex * @PageSize + 1

    IF @StartRowIndex <= @TotalCount
    BEGIN

        SET ROWCOUNT @StartRowIndex

        SELECT  
            @FirstTimeUTC = [TimeUtc],
            @FirstSequence = [Sequence]
        FROM 
            [ELMAH_Error]
        WHERE   
            [Application] = @Application
        ORDER BY 
            [TimeUtc] DESC, 
            [Sequence] DESC

    END
    ELSE
    BEGIN

        SET @PageSize = 0

    END

    -- Now set the row count to the requested page size and get
    -- all records below it for the pertaining application.

    SET ROWCOUNT @PageSize

    SELECT 
        errorId     = [ErrorId], 
        application = [Application],
        host        = [Host], 
        type        = [Type],
        source      = [Source],
        message     = [Message],
        [user]      = [User],
        statusCode  = [StatusCode], 
        time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
    FROM 
        [ELMAH_Error] error
    WHERE
        [Application] = @Application
    AND
        [TimeUtc] <= @FirstTimeUTC
    AND 
        [Sequence] <= @FirstSequence
    ORDER BY
        [TimeUtc] DESC, 
        [Sequence] DESC
    FOR
        XML AUTO




GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorXml]
(
    @Application NVARCHAR(60),
    @ErrorId UNIQUEIDENTIFIER
)
AS

    SET NOCOUNT ON

    SELECT 
        [AllXml]
    FROM 
        [ELMAH_Error]
    WHERE
        [ErrorId] = @ErrorId
    AND
        [Application] = @Application




GO
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [ELMAH_Error]
        (
            [ErrorId],
            [Application],
            [Host],
            [Type],
            [Source],
            [Message],
            [User],
            [AllXml],
            [StatusCode],
            [TimeUtc]
        )
    VALUES
        (
            @ErrorId,
            @Application,
            @Host,
            @Type,
            @Source,
            @Message,
            @User,
            @AllXml,
            @StatusCode,
            @TimeUtc
        )




GO
/****** Object:  StoredProcedure [dbo].[GetUserContactDetails_sp]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetUserContactDetails_sp]
@LicenseID int,
@StatusName varchar(50), 
@RoleName varchar(500),
@EmailAddress varchar(100) output
as
begin
	set nocount on

	
	declare @RegionId int,
			@StatusId int,
			@Role varchar(100),
			@UserId int,
			@DepartmentContactId int,
			@DepartmentHeadContactId int



	if @StatusName= 'License Pending ' ---
	begin
	select 	@RegionId =	RegionId
from [dbo].[Licenses]
where LicenseId = @LicenseID 

select @EmailAddress =[EmailAddress]
from [dbo].[Users]
where [Region] = @RegionId AND [Role] = @RoleName
end

	else if @StatusName = 'Awaiting Manager Response' ---
	begin
	select 	@RegionId =	RegionId
from [dbo].[Licenses]
where LicenseId = @LicenseID 

select @EmailAddress =[EmailAddress]
from [dbo].[Users]
where [Region] = @RegionId AND [Role] = @RoleName
end

	else if @StatusName = 'Awaiting Manager Response' ---
	begin
	select 	@RegionId =	RegionId
from [dbo].[Licenses]
where LicenseId = @LicenseID 

select @EmailAddress =[EmailAddress]
from [dbo].[Users]
where [Region] = @RegionId AND [Role] = @RoleName
end

	else if @StatusName = 'Inspection Pending ' ---
	begin

	select @StatusId = StatusId
	from [dbo].[Status]
where StatusName = @StatusName

	select 	@DepartmentContactId =	DepartmentContactId
from [dbo].[InspectionRequests]
where LicenseId = @LicenseID And StatusId = @StatusId

	select 	@UserId =UserId
from [dbo].[DepartmentContacts]
where DepartmentContactId = @DepartmentContactId AND [RoleName] = @RoleName

select @EmailAddress =[EmailAddress]
from [dbo].[Users]
where UserId = @UserId
end

	else if @StatusName = 'Inspection Pending ' ---
	begin

	select @StatusId = StatusId
	from [dbo].[Status]
where StatusName = @StatusName

	select 	@DepartmentContactId =	DepartmentContactId
from [dbo].[InspectionRequests]
where LicenseId = @LicenseID And StatusId = @StatusId

if @RoleName ='Department Inspector'
begin

	select 	@UserId =UserId
from [dbo].[DepartmentContacts]
where DepartmentContactId = @DepartmentContactId AND [RoleName] = @RoleName
end

else if @RoleName ='Department Manager'

begin
	select 	@DepartmentHeadContactId =DepartmentHeadContactId
from [dbo].[DepartmentContacts]
where DepartmentContactId = @DepartmentContactId 

	select 	@UserId =UserId
from [dbo].[DepartmentContacts]
where DepartmentContactId = @DepartmentHeadContactId AND [RoleName] = @RoleName
end



select @EmailAddress =[EmailAddress]
from [dbo].[Users]
where UserId = @UserId
end

end
	

	




GO
/****** Object:  StoredProcedure [dbo].[NotRenewed_sp]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[NotRenewed_sp]
--@case_desc datetime output
as
begin
--- run using job on 31 jan every year
	
	declare 
		    @statusIdNotRenewed int,
			@statusIdPendingRenew int
		




select @statusIdNotRenewed = [StatusId] from [dbo].[Status] where [StatusKey] ='NotRenewed'
select @statusIdPendingRenew = [StatusId] from [dbo].[Status] where [StatusKey] ='PendingRenewal'


UPDATE [dbo].[Licenses]
   SET [StatusId] = @statusIdNotRenewed
where  [StatusId] = @statusIdPendingRenew

	

end


GO
/****** Object:  StoredProcedure [dbo].[PendingRenewal_sp]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[PendingRenewal_sp]

-- run via job on 01 November every year
--@case_desc datetime output
as
begin


	
	declare 
	       @statusIdApproved int,
		    @statusIdNotRenewed int,
			@statusIdPendingRenew int,
			@currentyear  int,
			@Nexttyear int,
			@Nov int,
			@Dec int

set @currentyear =	CAST(YEAR(getdate())* 100  as int)
set @Nexttyear =  CAST(YEAR(DATEADD(year, 1, getdate())) as int)
set @Nov =  CAST(@currentyear +'11' as  int)
set @Dec = CAST(@currentyear +'12' as  int)



select @statusIdNotRenewed = [StatusId] from [dbo].[Status] where [StatusKey] ='NotRenewed'
select @statusIdPendingRenew = [StatusId] from [dbo].[Status] where [StatusKey] ='PendingRenewal'
select @statusIdApproved = [StatusId] from [dbo].[Status] where [StatusKey] ='LicenseApproved'


UPDATE [dbo].[Licenses]
   SET [StatusId] = @statusIdPendingRenew
   where [StatusId] = @statusIdApproved
--where  CAST(YEAR(ApplicationDateTime) AS nvarchar ) !=  @Nexttyear And CAST(YEAR(ApplicationDateTime)  * 100 +'' + MONTH(ApplicationDateTime)AS nvarchar) !=  @Nov And CAST(YEAR(ApplicationDateTime)  * 100 +'' + MONTH(ApplicationDateTime)AS nvarchar) !=  @Dec
 


	

end


GO
/****** Object:  StoredProcedure [dbo].[RefusalProcess_sp]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RefusalProcess_sp]
--@case_desc int output
as
begin


	
	declare @currentdate date,
	@appdate datetime,
	@adjusteddate datetime,
      @SlaExpiryDate datetime,	
	    @LicenseTypeId int,	
	 @numDays INT = 21,
		  @ClientId int,	
		 @LicenseId int,
		 @DepartmentId int,
		 @DepartmentContactId int,
		 @InspectionRequestId int,
		 @InspectionResponseId int,
			@statusIdPendingLicense int,
			@statusIdAwaitingDeptManager int,
			@statusIdPendingLicenseInspection int,
			@statusIdLicenseRefusalInProgress int,
			@statusIdInspectionFailed int,
			@statusIdPendingRefusal int

set @currentdate =  CAST(getdate()   AS DATE)
set  @SlaExpiryDate = CAST(dateadd(month,3,@currentdate) AS DATE)

select @statusIdPendingLicense= [StatusId] from [dbo].[Status] where [StatusKey] ='LicensePending'
select @statusIdAwaitingDeptManager = [StatusId] from [dbo].[Status] where [StatusKey] ='AwaitingManagerResponse'
select @statusIdPendingLicenseInspection= [StatusId] from [dbo].[Status] where [StatusKey] ='PendingLicenseInspection'
select @statusIdLicenseRefusalInProgress = [StatusId] from [dbo].[Status] where [StatusKey] ='RefusalInProgress'
select @statusIdPendingRefusal= [StatusId] from [dbo].[Status] where [StatusKey] ='PendingRefusal'
select @statusIdInspectionFailed = [StatusId] from [dbo].[Status] where [StatusKey] ='InspectionFailed'



/******LicensePending status start ******/
DECLARE @LicensePendingCursor CURSOR
BEGIN
  
    SET @LicensePendingCursor = CURSOR FOR
    SELECT LicenseId FROM [dbo].[Licenses] where  [StatusId]= @statusIdPendingLicense And [IsActive] = 'True' 
	--SELECT LicenseId FROM [dbo].[Licenses] where [StatusId]= @statusIdPendingLicense
	 
    OPEN @LicensePendingCursor 
    FETCH NEXT FROM @LicensePendingCursor 
    INTO @LicenseId
	
    WHILE @@FETCH_STATUS = 0

    BEGIN
	set @numDays = 21
	select  @appdate = [CreatedDateTime]  from [dbo].[Licenses] where LicenseId = @LicenseId 
	


	 WHILE @numDays > 0
    BEGIN
	
       SET @adjusteddate=CAST(dateadd(day,1,@appdate)as date)

       IF DATENAME(day,@adjusteddate)='saturday'
	    BEGIN

	    SET @adjusteddate=dateadd(day,1,@adjusteddate)
		 END
       IF DATENAME(day,@adjusteddate)='sunday'
	    BEGIN
		 SET @adjusteddate=dateadd(day,1,@adjusteddate)
		  END
  
       SET @numDays=@numDays-1
	   	    
    END
	 
	 IF @adjusteddate = @currentdate 
 BEGIN
	--change license status
	     UPDATE [dbo].[Licenses]
   SET [StatusId] = @statusIdLicenseRefusalInProgress 
   where LicenseId = @LicenseId

   select  @LicenseTypeId = [LicenseTypeId] from [dbo].[Licenses] where LicenseId = @LicenseId
      select   @ClientId  = [ClientId] from [dbo].[Licenses] where LicenseId = @LicenseId
	  select @DepartmentId = [DepartmentId] from [dbo].[Departments] where [DepartmentKey] ='ETK1010'
	  select @DepartmentContactId = [DepartmentContactId] from [dbo].[DepartmentContacts] where [DepartmentId] =@DepartmentId and RoleName ='Department Inspector'
	  --add an inspection request
   INSERT INTO [dbo].[InspectionRequests]
           ([OverrideOrderIndex]
           ,[LicenseId]
           ,[LicenseTypeId]
           ,[DepartmentId]
           ,[ClientId]
           ,[StatusId]
           ,[SlaExpiryDate]
           ,[IsActive]
           ,[IsDeleted]
           ,[IsLocked]
           ,[CreatedDateTime]
           ,[RefNumber]
           ,[AllocatedDate]
		   ,[Comment]
           ,[DepartmentContactId]
        )
     VALUES
           ('False'
           ,@LicenseId
           ,@LicenseTypeId
           ,@DepartmentId
           ,@ClientId 
           ,@statusIdInspectionFailed
           ,@SlaExpiryDate
           ,'False'
           ,'True'
           ,'False'
           ,@currentdate
		   ,'Ref'+ CAST(@LicenseId AS nvarchar)
           ,@currentdate
		   ,'system generated'
 ,@DepartmentContactId
          )
		  --add an inspection
		 SELECT TOP 1 @InspectionRequestId  = [InspectionRequestId] from [dbo].[InspectionRequests] where LicenseId = @LicenseId ORDER BY InspectionRequestId DESC
		
		  INSERT INTO [dbo].[InspectionResponses]
           ([InspectionRequestId]
      
           ,[LicenseId]
           ,[Comment]
           ,[StatusId]
           ,[IsActive]
           ,[IsDeleted]
           ,[IsLocked]
           ,[CreatedDateTime]
           ,[InspectionDateTime]
           ,[InspectionFailureCount]
         
          )
     VALUES
           (@InspectionRequestId
           ,@LicenseId
           ,'system generated'
           ,@statusIdInspectionFailed
           ,'True'
           ,'False'
           ,'False'
           ,@currentdate
           ,@currentdate
           ,'1'

           )
		    SELECT TOP 1 @InspectionResponseId  = [InspectionResponseId] from [dbo].[InspectionResponses] where LicenseId = @LicenseId ORDER BY InspectionResponseId DESC
INSERT INTO [dbo].[InspectionHistories]
         ([InspectionRequestId]
           ,[InspectionResponseId]
           ,[InspectionDateTime]
           ,[InspectionFailureCount]
           ,[LicenseId]
           ,[Comment]
           ,[StatusId]
		  ,[CapturedByClerk]
		   ,[AdhocInspector]
          )
     VALUES
           (@InspectionRequestId
           ,@InspectionResponseId
           ,@currentdate
           ,'1'
           ,@LicenseId
           ,'system generated'
           ,@statusIdInspectionFailed
		   ,'False'
		   ,'False'
           )
END;
      FETCH NEXT FROM @LicensePendingCursor 
      INTO @LicenseId
	 
    END; 

    CLOSE @LicensePendingCursor ;
    DEALLOCATE @LicensePendingCursor;
END;
   



/******LicensePending status end ******/

/******Awaiting Manager Response status start ******/
DECLARE @AwaitingDeptManagerCursor CURSOR
BEGIN
 
    SET @AwaitingDeptManagerCursor = CURSOR FOR
      SELECT InspectionRequestId FROM [dbo].[InspectionRequests] where[StatusId]= @statusIdAwaitingDeptManager
	--SELECT InspectionRequestId FROM [dbo].[InspectionRequests] where [StatusId]= @statusIdAwaitingDeptManager

    OPEN @AwaitingDeptManagerCursor 
    FETCH NEXT FROM @AwaitingDeptManagerCursor
    INTO @InspectionRequestId 
	
    WHILE @@FETCH_STATUS = 0
    BEGIN
	set @numDays = 21
	select  @appdate = [CreatedDateTime]  from [dbo].[InspectionRequests] where InspectionRequestId = @InspectionRequestId 
	


	 WHILE @numDays > 0
    BEGIN
	
       SET @adjusteddate=CAST(dateadd(day,1,@appdate)as date)

       IF DATENAME(day,@adjusteddate)='saturday'
	    BEGIN

	    SET @adjusteddate=dateadd(day,1,@adjusteddate)
		 END
       IF DATENAME(day,@adjusteddate)='sunday'
	    BEGIN
		 SET @adjusteddate=dateadd(day,1,@adjusteddate)
		  END
  
       SET @numDays=@numDays-1
	   	    
    END
	 
	 IF @adjusteddate = @currentdate 
 BEGIN
	SELECT @LicenseId  = [LicenseId] from [dbo].[InspectionRequests] where InspectionRequestId = @InspectionRequestId 
   select @DepartmentId = [DepartmentId] from  [dbo].[InspectionRequests] where InspectionRequestId = @InspectionRequestId 
  SELECT TOP 1 @DepartmentContactId = [DepartmentContactId] from [dbo].[DepartmentContacts] where [DepartmentId] =@DepartmentId and RoleName ='Department Inspector' ORDER BY DepartmentId DESC
	  --Update an inspection request
	  UPDATE [dbo].[InspectionRequests]
   SET 
      [StatusId] = @statusIdInspectionFailed
      ,[IsActive] = 'False'
      ,[IsDeleted] = 'True'
      ,[IsLocked] = 'False'
      ,[ModifiedDateTime] = @currentdate
      ,[Comment] = 'system generated'
      ,[DepartmentContactId] = @DepartmentContactId
 WHERE InspectionRequestId = @InspectionRequestId 

     
		  --add an inspection
		
		
		  INSERT INTO [dbo].[InspectionResponses]
           ([InspectionRequestId]
      
           ,[LicenseId]
           ,[Comment]
           ,[StatusId]
           ,[IsActive]
           ,[IsDeleted]
           ,[IsLocked]
           ,[CreatedDateTime]
           ,[InspectionDateTime]
           ,[InspectionFailureCount]
         
          )
     VALUES
           (@InspectionRequestId
           ,@LicenseId
           ,'system generated'
           ,@statusIdInspectionFailed
           ,'True'
           ,'False'
           ,'False'
           ,@currentdate
           ,@currentdate
           ,'1'

           )
		    SELECT TOP 1 @InspectionResponseId  = [InspectionResponseId] from [dbo].[InspectionResponses] where LicenseId = @LicenseId ORDER BY InspectionResponseId DESC
INSERT INTO [dbo].[InspectionHistories]
         ([InspectionRequestId]
           ,[InspectionResponseId]
           ,[InspectionDateTime]
           ,[InspectionFailureCount]
           ,[LicenseId]
           ,[Comment]
           ,[StatusId]
		  ,[CapturedByClerk]
		   ,[AdhocInspector]
          )
     VALUES
           (@InspectionRequestId
           ,@InspectionResponseId
           ,@currentdate
           ,'1'
           ,@LicenseId
           ,'system generated'
           ,@statusIdInspectionFailed
		   ,'False'
		   ,'False'
           )
		   --change license status
	 
	     UPDATE [dbo].[Licenses]
   SET [StatusId] = @statusIdLicenseRefusalInProgress 
   where LicenseId = @LicenseId
 END; 
      FETCH NEXT FROM @AwaitingDeptManagerCursor 
      INTO @InspectionRequestId 
	 
    END; 

    CLOSE @AwaitingDeptManagerCursor ;
    DEALLOCATE @AwaitingDeptManagerCursor;
END;
   



/******Awaiting Manager Response status end ******/

/******Pending License Inspection status start ******/
DECLARE @PendingLicenseInspectionCursor CURSOR
BEGIN
 
    SET @PendingLicenseInspectionCursor = CURSOR FOR
   SELECT InspectionRequestId FROM [dbo].[InspectionRequests] where  [StatusId]= @statusIdPendingLicenseInspection
	--SELECT InspectionRequestId FROM [dbo].[InspectionRequests] where [StatusId]= @statusIdPendingLicenseInspection

    OPEN @PendingLicenseInspectionCursor 
    FETCH NEXT FROM @PendingLicenseInspectionCursor
    INTO @InspectionRequestId 
	
    WHILE @@FETCH_STATUS = 0
    BEGIN
	 
	set @numDays = 21
	select  @appdate = [ModifiedDateTime]  from [dbo].[InspectionRequests] where InspectionRequestId = @InspectionRequestId 
	


	 WHILE @numDays > 0
    BEGIN
	
       SET @adjusteddate=CAST(dateadd(day,1,@appdate)as date)

       IF DATENAME(day,@adjusteddate)='saturday'
	    BEGIN

	    SET @adjusteddate=dateadd(day,1,@adjusteddate)
		 END
       IF DATENAME(day,@adjusteddate)='sunday'
	    BEGIN
		 SET @adjusteddate=dateadd(day,1,@adjusteddate)
		  END
  
       SET @numDays=@numDays-1
	   	    
    END
	 
	 IF @adjusteddate = @currentdate 
 BEGIN
	SELECT @LicenseId  = [LicenseId] from [dbo].[InspectionRequests] where InspectionRequestId = @InspectionRequestId 
   --Update an inspection request
	  UPDATE [dbo].[InspectionRequests]
   SET 
      [StatusId] = @statusIdInspectionFailed
      ,[IsActive] = 'False'
      ,[IsDeleted] = 'True'
      ,[IsLocked] = 'False'
      ,[ModifiedDateTime] = @currentdate
 WHERE InspectionRequestId = @InspectionRequestId 
		  --add an inspection

		  INSERT INTO [dbo].[InspectionResponses]
           ([InspectionRequestId]
      
           ,[LicenseId]
           ,[Comment]
           ,[StatusId]
           ,[IsActive]
           ,[IsDeleted]
           ,[IsLocked]
           ,[CreatedDateTime]
           ,[InspectionDateTime]
           ,[InspectionFailureCount]
         
          )
     VALUES
           (@InspectionRequestId
           ,@LicenseId
           ,'system generated'
           ,@statusIdInspectionFailed
           ,'True'
           ,'False'
           ,'False'
           ,@currentdate
           ,@currentdate
           ,'1'

           )
		    SELECT TOP 1 @InspectionResponseId  = [InspectionResponseId] from [dbo].[InspectionResponses] where LicenseId = @LicenseId ORDER BY InspectionResponseId DESC
INSERT INTO [dbo].[InspectionHistories]
         ([InspectionRequestId]
           ,[InspectionResponseId]
           ,[InspectionDateTime]
           ,[InspectionFailureCount]
           ,[LicenseId]
           ,[Comment]
           ,[StatusId]
		  ,[CapturedByClerk]
		   ,[AdhocInspector]
          )
     VALUES
           (@InspectionRequestId
           ,@InspectionResponseId
           ,@currentdate
           ,'1'
           ,@LicenseId
           ,'system generated'
           ,@statusIdInspectionFailed
		   ,'False'
		   ,'False'
           )
		   --change license status
	 
	     UPDATE [dbo].[Licenses]
   SET [StatusId] = @statusIdLicenseRefusalInProgress 
   where LicenseId = @LicenseId
    END; 

      FETCH NEXT FROM @PendingLicenseInspectionCursor 
      INTO @InspectionRequestId 
	 
    END; 

    CLOSE @PendingLicenseInspectionCursor ;
    DEALLOCATE @PendingLicenseInspectionCursor;
END;
   



/******Pending License Inspection status end ******/

--select @create = [CreatedDateTime] from [dbo].[Licenses] where [LicenseId] = '18493'
--set  @show  = CAST(dateadd(day,21,@create) AS DATE)

--select @case_desc=  @InspectionRequestId 
	

end

GO
/****** Object:  StoredProcedure [dbo].[SCRIPT_MagrationTradeLicenseSystem]    Script Date: 2/11/2021 1:07:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Original Author 
-- Create date: <Create Date,,20200331>
-- Description:	<Description,, Migrates legacy data to the new trade license system>
-- =============================================
CREATE PROCEDURE [dbo].[SCRIPT_MagrationTradeLicenseSystem]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BEGIN TRANSACTION;

BEGIN TRY
DECLARE Client_cursor CURSOR LOCAL FOR 
SELECT DISTINCT LTRIM(RTRIM(Proprietor)) FROM _LicensesCentral;

DECLARE @Proprietor VARCHAR(MAX);
OPEN  Client_cursor

FETCH NEXT FROM Client_cursor
	INTO @Proprietor

WHILE @@FETCH_STATUS = 0
BEGIN
			-- Create client
			DECLARE @Clients TABLE(
				[ClientId] [int] IDENTITY(1,1) NOT NULL,
				[IdentityOrPassportNumber] [nvarchar](max) NOT NULL,
				[Name] [nvarchar](max) NOT NULL,
				[Surname] [nvarchar](max) NOT NULL,
				[ResidentialAddress1] [nvarchar](max) NULL,
				[ResidentialAddress2] [nvarchar](max) NULL,
				[ResidentialAddress3] [nvarchar](max) NULL,
				[ResidentialAddressCode] [int] NOT NULL,
				[PostalAddress1] [nvarchar](max) NULL,
				[PostalAddress2] [nvarchar](max) NULL,
				[PostalAddress3] [nvarchar](max) NULL,
				[PostalAddressCode] [int] NOT NULL,
				[TelephoneNumber] [nvarchar](max) NULL,
				[CellphoneNumber] [nvarchar](max) NULL,
				[FaxNumber] [nvarchar](max) NULL,
				[EmailAddress] [nvarchar](max) NULL,
				[ClientStatusId] [int] NOT NULL,
				[IsActive] [bit] NOT NULL,
				[IsDeleted] [bit] NOT NULL,
				[IsLocked] [bit] NOT NULL,
				[CreatedByUserId] [int] NULL,
				[CreatedDateTime] [datetime] NULL,
				[ModifiedByUserId] [int] NULL,
				[ModifiedDateTime] [datetime] NULL)

				INSERT INTO MigratedClients(
					[Name],
					[Surname],
					[ResidentialAddress1],
					[ResidentialAddress2],
					[ResidentialAddressCode],
					[PostalAddress1],
					[PostalAddress2],
					[PostalAddress3],
					[PostalAddressCode],
					[CellphoneNumber],
					[EmailAddress],
					[ClientStatusId],
					[IsActive],
					[IsDeleted],
					[IsLocked])
			  SELECT TOP 1
					Proprietor ClientName
					,'' Surname
					,PhysicalAddress1
					,PhysicalAddress2
					,0 ResidentialAddressCode
					,[Postal1]
					,[Postal2]
					,[Postal3]
					,0 PostalAddressCode
					,CASE WHEN Telephone IS NOT NULL  THEN Telephone ELSE '' END  CellPhone
					,'' EmailAddress
					,13 ClientStatusId
					,1 IsActive
					,0 IsDeleted
					,0 IsLocked 
			  FROM 
					_LicensesCentral l
					LEFT JOIN Clients c ON l.Proprietor = c.[Name]
			  WHERE
					Proprietor =  @Proprietor
					AND c.ClientId IS NULL;

			DECLARE @ClientId INT;
			SET @ClientId = @@IDENTITY;

			 PRINT @ClientId;

			--Gets operator details
			DECLARE @OperatorName VARCHAR(MAX);
			DECLARE @OperatorIdentityNumber VARCHAR(MAX);
			SET @OperatorName = (SELECT DISTINCT Name FROM Clients WHERE ClientId = @ClientId);
			SET @OperatorIdentityNumber = (SELECT DISTINCT IdentityOrPassportNumber FROM Clients WHERE ClientId =@ClientId);

			DECLARE Bus_Cursor CURSOR LOCAL FOR
			SELECT DISTINCT BusinessName FROM _LicensesCentral WHERE Proprietor = @Proprietor

			OPEN Bus_Cursor
			DECLARE @BusinessName VARCHAR(MAX);
			--Create Business
			DECLARE @Businesses TABLE(
					[BusinessId] [int] IDENTITY(1,1) NOT NULL,
					[ClientId] [int] NOT NULL,
					[ProposedTradeName] [nvarchar](max) NOT NULL,
					[BusinessTypeId] [int] NOT NULL,
					[PostalAddress1] [nvarchar](max) NULL,
					[PostalAddress2] [nvarchar](max) NULL,
					[PostalAddress3] [nvarchar](max) NULL,
					[PostalAddressCode] [int] NOT NULL,
					[ResidentialShopOrUnitNumber] [int] NOT NULL,
					[ResidentialStreetAddress] [nvarchar](max) NULL,
					[ResidentialAddress1] [nvarchar](max) NULL,
					[ResidentialAddress2] [nvarchar](max) NULL,
					[ResidentialAddress3] [nvarchar](max) NULL,
					[ResidentialAddressCode] [int] NOT NULL,
					[TelephoneNumber] [nvarchar](max) NULL,
					[CellphoneNumber] [nvarchar](max) NULL,
					[FaxNumber] [nvarchar](max) NULL,
					[TitleDeedTypeId] [int] NOT NULL,
					[NameOfBusinessOperator] [nvarchar](max) NULL,
					[OperatorIdentityOrPassportNumber] [nvarchar](max) NULL,
					[OperatorResidentialAddress1] [nvarchar](max) NULL,
					[OperatorResidentialAddress2] [nvarchar](max) NULL,
					[OperatorResidentialAddress3] [nvarchar](max) NULL,
					[OperatorResidentialAddressCode] [int] NOT NULL,
					[NameOfEmployer] [nvarchar](max) NULL,
					[EmployerResidentialAddress1] [nvarchar](max) NULL,
					[EmployerResidentialAddress2] [nvarchar](max) NULL,
					[EmployerResidentialAddress3] [nvarchar](max) NULL,
					[EmployerResidentialAddressCode] [int] NOT NULL,
					[OperationStructureTypeId] [int] NOT NULL,
					[BusinessStatusId] [int] NOT NULL,
					[IsActive] [bit] NOT NULL,
					[IsDeleted] [bit] NOT NULL,
					[IsLocked] [bit] NOT NULL,
					[CreatedByUserId] [int] NULL,
					[CreatedDateTime] [datetime] NULL,
					[ModifiedByUserId] [int] NULL,
					[ModifiedDateTime] [datetime] NULL,
					[BusinessOperatorId] [int] NULL,
					[RatesAccountNumber] [nvarchar](max) NULL,
					[ItemTypeId] [int] NULL);

			FETCH NEXT FROM Bus_Cursor
			INTO @BusinessName

			WHILE @@FETCH_STATUS = 0
				BEGIN
					INSERT INTO MigratedBusinesses(
							[MClientId],
							[ProposedTradeName],
							[BusinessTypeId],
							[PostalAddress1],
							[PostalAddress2],
							[PostalAddress3],
							[PostalAddressCode],
							[ResidentialShopOrUnitNumber],
							[ResidentialAddress1],
							[ResidentialAddress2],
							[ResidentialAddressCode],
							[TelephoneNumber],
							[TitleDeedTypeId],
							[BusinessOperatorId],
							[NameOfBusinessOperator],
							[OperatorIdentityOrPassportNumber],
							[OperatorResidentialAddress1],
							[OperatorResidentialAddress2],
							[OperatorResidentialAddressCode],
							[NameOfEmployer],
							[EmployerResidentialAddressCode],
							[ItemTypeId],
							[OperationStructureTypeId],
							[BusinessStatusId],
							[IsSelfEmployed],
							[KnownAs],
							[IsActive],
							[IsDeleted],
							[IsLocked])
					SELECT	TOP 1
							@ClientId,
							BusinessName,
							1 As BusinessTypeId,
							CASE WHEN Postal1 IS NULL THEN 'N/A' ELSE Postal1 END PostalAddress1,
							CASE WHEN Postal2 IS NULL THEN 'N/A' ELSE Postal2 END PostalAddress1,
							Postal3,
							0 As PostalAddressCode,
							0 As ResidentialShopOrUnitNumber,
							CASE WHEN PhysicalAddress1 IS NULL THEN 'N/A' ELSE PhysicalAddress1 END PhysicalAddress1,
							CASE WHEN PhysicalAddress2 IS NULL THEN 'N/A' ELSE PhysicalAddress2 END PhysicalAddress2,
							0 As ResidentialAddressCode,
							Telephone,
							1 As TitleDeedTypeId,
							5 As BusinessOperatorId,
							@OperatorName As NameOfBusinessOperator,
							@OperatorIdentityNumber As OperatorIdentityOrPassportNumber,
							'' OAddress1,
							'' OAddress2,
							0 As OperatorResidentialAddressCode,
							Employer,
							0 As EmployerResidentialAddressCode,
							CASE WHEN LicenseType ='A' THEN 1002 --Accommodation
									   WHEN LicenseType = 1 THEN 1 --Sale or Supply
									   WHEN LicenseType = 2 THEN 3 --Health & Entertainment
									   ELSE 4--Hawking
								  END As ItemTypeId,
							1 As OperationStructureTypeId,
							16 As BusinessStatusId,
							CASE WHEN Employer IS NULL THEN 1 ELSE 0 END As IsSelfEmployed,
							TradingName KnownAs,
							1 As IsActive,
							0 As IsDeleted,
							0 As IsLocked 
					FROM	
							_LicensesCentral l
							LEFT JOIN Businesses b ON l.BusinessName = b.ProposedTradeName
					WHERE
							BusinessName = @BusinessName
							AND b.BusinessId IS NULL;

					--Print('Propriator: '+@Proprietor +' BusinessName: '+ @BusinessName)
					DECLARE @BusinessId INT;
					SET @BusinessId =@@IDENTITY;

					DECLARE License_cursor CURSOR LOCAL FOR
					SELECT DISTINCT RefNo FROM _LicensesCentral WHERE BusinessName =@BusinessName;
				
					OPEN License_cursor
					DECLARE @RefNo VARCHAR(25);
					DECLARE @Licenses TABLE(
							[LicenseId] [int] IDENTITY(1,1) NOT NULL,
							[LicenseTypeId] [int] NOT NULL,
							[ClientId] [int] NULL,
							[BusinessId] [int] NULL,
							[LicenseIssueYear] [int] NOT NULL,
							[ApplicationDateTime] [datetime] NOT NULL,
							[LicenseIssueDateTime] [datetime] NULL,
							[NotificationUpdatedDateTime] [datetime] NULL,
							[StatusId] [int] NOT NULL,
							[LicenseClosureDateTime] [datetime] NULL,
							[LicenseCollectedDateTime] [datetime] NULL,
							[RequiredDocuments] [bit] NOT NULL,
							[IsActive] [bit] NOT NULL,
							[IsDeleted] [bit] NOT NULL,
							[IsLocked] [bit] NOT NULL,
							[CreatedByUserId] [int] NULL,
							[CreatedDateTime] [datetime] NULL,
							[ModifiedByUserId] [int] NULL,
							[ModifiedDateTime] [datetime] NULL,
							[LicenseExpiryDate] [datetime] NULL,
							[LicenseNumber] [nvarchar](max) NULL,
							[ItemTypeId] [int] NULL,
							[ItemSubCategoryId] [int] NOT NULL,
							[RegionId] [int] NOT NULL,
							[ItemConditionId] [int] NOT NULL,
							[LicenseRenewalDateTime] [datetime] NULL);

					FETCH NEXT FROM License_cursor
					INTO @RefNo

					WHILE @@FETCH_STATUS = 0
						BEGIN
							INSERT INTO MigratedLicenses(
								[LicenseTypeId],
								[MClientId],
								[MBusinessId],
								[LicenseIssueYear],
								[ApplicationDateTime],
								[LicenseIssueDateTime],
								[NotificationUpdatedDateTime],
								[StatusId],
								[IsActive],
								[IsDeleted],
								[IsLocked],
								[LicenseNumber],
								[LicenseExpiryDate],
								[ItemTypeId],
								[ItemSubCategoryId],
								[RegionId],
								[ItemConditionId])
							SELECT
								t.LicenseTypeId As LicenseTypeId,
								@ClientId,
								@BusinessId,
								CASE WHEN ISDATE(LicenseIssueDate)=1 THEN DATEPART(Year,LicenseIssueDate) END As LicenseIssueYear,
								--CASE WHEN  ISDATE(LicenseIssueDate)=1 THEN CAST(LicenseIssueDate As datetime) END 
								null As ApplicationDateTime,								   
								CASE WHEN  ISDATE(LicenseIssueDate)=1 THEN CAST(LicenseIssueDate As datetime) END As LicenseIssueDateTime ,
								CASE WHEN  ISDATE([CurrentDate])=1 THEN CAST([CurrentDate] As datetime) END As NotificationUpdatedDateTime, 
								CASE WHEN PendingIndicator=0 THEN 3 ELSE 4 END As StatusId, -- Approved 
								1 As IsActive,
								0 As IsDeleted,
								0 AS IsLocked,
								RefNo As LicenseNumber,--@RefNo
								CASE WHEN  ISDATE(LicenseIssueDate)=1 THEN DATEADD(YEAR,1,CAST(LicenseIssueDate As datetime)) END AS LicenseExpiryDate,
								t.ItemTypeId As ItemTypeId,
								CASE WHEN l.LicenseType ='A' THEN 
									CASE WHEN LicenseSubType='/' THEN 18 --N/A
										WHEN LicenseSubType='A' THEN 8
										ELSE 18
									END
									WHEN l.LicenseType = 1 THEN --Sale or Supply
									CASE WHEN LicenseSubType='/' THEN 15 --N/A 
										WHEN LicenseSubType='1' THEN 21
										WHEN LicenseSubType ='A' THEN 23
										WHEN LicenseSubType ='I' THEN 22
										WHEN LicenseSubType ='J' THEN 20
									END
									WHEN l.LicenseType = 2 THEN  --Health & Entertainment
									CASE WHEN LicenseSubType='F' THEN 1
										WHEN LicenseSubType='D' THEN 24
										WHEN LicenseSubType='G' THEN 4
										WHEN LicenseSubType='H' THEN 3
										WHEN LicenseSubType='A' THEN 8
										WHEN LicenseSubType='B' THEN 9
										WHEN LicenseSubType='E' THEN 25
										WHEN LicenseSubType='1' THEN 26
										--WHEN LicenseSubType='2' THEN 19
										ELSE 16
									END
									ELSE 
									CASE WHEN LicenseSubType='/' THEN 17 --Hawking 
										WHEN LicenseSubType='1' THEN 2
										WHEN LicenseSubType='E' THEN 27
										WHEN LicenseSubType='J' THEN 29
										WHEN LicenseSubType ='I' THEN 28
									END
								END  As ItemSubCategoryId,
								(SELECT RegionId FROM Regions WHERE RegionKey ='metro_central') As RegionId,
								(SELECT TOP 1 ItemConditionId FROM ItemConditions WHERE ItemTypeId =(
								SELECT CASE WHEN l.LicenseType ='A' THEN 6 --Accommodation
									WHEN l.LicenseType = 1 THEN 3 --Sale or Supply
									WHEN l.LicenseType = 2 THEN 2 --Health & Entertainment
									ELSE 5--Hawking
								END)) As ItemConditionId
							FROM 
								_LicensesCentral l 
								INNER JOIN _LicenseTypes t ON l.LicenseType = t.LicenseType
								LEFT JOIN Licenses l2 ON l.RefNo = l2.LicenseNumber
							WHERE
								l.RefNo =@RefNo
								AND l2.LicenseId IS NULL

						--Fetch next business license
						FETCH NEXT FROM License_cursor
						INTO @RefNo

						END

						CLOSE License_cursor
						DEALLOCATE License_cursor

					-- Fetch Next Business
					FETCH NEXT FROM Bus_Cursor
					INTO @BusinessName

				END

			CLOSE Bus_Cursor
			DEALLOCATE Bus_Cursor

		--Fetch Next Client	
		FETCH NEXT FROM Client_cursor 
		INTO @Proprietor
		END

	CLOSE Client_cursor
	DEALLOCATE Client_cursor

-- View data to be migrated
--SELECT 	* FROM @Users;
--SELECT 	* FROM @Clients;
--SELECT 	* FROM @Businesses;
--SELECT 	* FROM @Licenses;
--@Users u
--INNER JOIN @Clients c ON u.UserId =c.ClientUserId 
--INNER JOIN @Businesses b ON c.ClientId =b.ClientId 
--LEFT JOIN @Licenses l ON b.BusinessId =l.BusinessId
-- SELECT * FROM _LicensesCentral
END TRY
BEGIN CATCH
	SELECT 
		ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION;
END CATCH;

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION;
END



GO
