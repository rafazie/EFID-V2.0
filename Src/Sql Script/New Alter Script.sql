/****** Object:  Table [dbo].[M_GateReview]    Script Date: 24/06/2024 08:09:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_GateReview](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Jabatan] [varchar](50) NULL,
	[Fungsi] [varchar](150) NULL
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[M_GateReview] ON 
GO
INSERT [dbo].[M_GateReview] ([ID], [Jabatan], [Fungsi]) VALUES (2, N'Manager', N'Corporate Secretary & Legal')
GO
INSERT [dbo].[M_GateReview] ([ID], [Jabatan], [Fungsi]) VALUES (3, N'Manager', N'Corporate Strategic Planning & Risk Management')
GO
INSERT [dbo].[M_GateReview] ([ID], [Jabatan], [Fungsi]) VALUES (4, N'Manager', N'Finance Business Support')
GO
INSERT [dbo].[M_GateReview] ([ID], [Jabatan], [Fungsi]) VALUES (5, N'Manager', N'Business Planning & Development')
GO
SET IDENTITY_INSERT [dbo].[M_GateReview] OFF
GO

/****** Object:  Table [dbo].[M_EmailManager]    Script Date: 24/06/2024 08:13:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[M_EmailManager](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Jabatan] [varchar](100) NULL,
	[Fungsi] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[NIP] [varchar](20) NULL,
	[Username] [varchar](20) NULL,
 CONSTRAINT [PK_M_EmailManager] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[M_EmailManager] ON 
GO
INSERT [dbo].[M_EmailManager] ([Id], [Jabatan], [Fungsi], [Email], [NIP], [Username]) VALUES (493, N'Manager', N'Corporate Secretary & Legal', N'ardhi.widodo@pertamina.com', N'39080015', N'ardhi.widodo')
GO
INSERT [dbo].[M_EmailManager] ([Id], [Jabatan], [Fungsi], [Email], [NIP], [Username]) VALUES (494, N'Manager', N'Corporate Strategic Planning & Risk Management', N'sutanty.sari@pertamina.com', N'39080009', N'sbsari')
GO
INSERT [dbo].[M_EmailManager] ([Id], [Jabatan], [Fungsi], [Email], [NIP], [Username]) VALUES (495, N'Manager', N'Finance Business Support', N'osep.gunawan@pertamina.com', N'728096', N'osep.gunawan')
GO
INSERT [dbo].[M_EmailManager] ([Id], [Jabatan], [Fungsi], [Email], [NIP], [Username]) VALUES (496, N'Manager', N'Business Planning & Development', N'ira.hidayana@pertamina.com', N'39080005', N'ihidayana')
GO
SET IDENTITY_INSERT [dbo].[M_EmailManager] OFF
GO


SET IDENTITY_INSERT [dbo].[M_Status] ON 
GO
INSERT [dbo].[M_Status] ([statusID], [status], [value], [SlaDefault]) VALUES (16, 21, N'<span> <i class="fa fa-pencil-square-o" aria-hidden="true" style="color:green;"></i> Approval Doc. 1 </span>', NULL)
GO
INSERT [dbo].[M_Status] ([statusID], [status], [value], [SlaDefault]) VALUES (17, 22, N'<span> <i class="fa fa-pencil-square-o" aria-hidden="true" style="color:green;"></i> Approval Doc. 2 </span>', NULL)
GO
INSERT [dbo].[M_Status] ([statusID], [status], [value], [SlaDefault]) VALUES (18, 23, N'<span> <i class="fa fa-pencil-square-o" aria-hidden="true" style="color:green;"></i> Approval Doc. FID 1  </span>', NULL)
GO
INSERT [dbo].[M_Status] ([statusID], [status], [value], [SlaDefault]) VALUES (19, 24, N'<span> <i class="fa fa-pencil-square-o" aria-hidden="true" style="color:green;"></i> Approval Doc. FID 2  </span>', NULL)
GO
SET IDENTITY_INSERT [dbo].[M_Status] OFF
GO


ALTER TABLE M_EMAIL
ADD CreatedBy VARCHAR(50),
CreatedDate datetime
GO

ALTER TABLE M_UploadFiles
ADD Category VARCHAR(50),
HtmlTag VARCHAR(MAX),
UploadBy VARCHAR(50),
UploadDate DATETIME,
StatusSign BIT
GO