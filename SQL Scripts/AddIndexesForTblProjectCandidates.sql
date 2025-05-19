USE [DSS]
GO

/****** Object:  Index [IX_TblProjectCandidates_PoolId]    Script Date: 7/4/2024 1:42:48 PM ******/
DROP INDEX [IX_TblProjectCandidates_PoolId] ON [dbo].[TblProjectCandidates]
GO

/****** Object:  Index [IX_TblProjectCandidates_PoolId]    Script Date: 7/4/2024 1:42:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_TblProjectCandidates_PoolId] ON [dbo].[TblProjectCandidates]
(
	[PoolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO


/****** Object:  Index [IX_TblProjectCandidates_Description]    Script Date: 7/4/2024 1:43:04 PM ******/
DROP INDEX [IX_TblProjectCandidates_Description] ON [dbo].[TblProjectCandidates]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_TblProjectCandidates_Description]    Script Date: 7/4/2024 1:43:04 PM ******/
CREATE NONCLUSTERED INDEX [IX_TblProjectCandidates_Description] ON [dbo].[TblProjectCandidates]
(
	[Description] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

