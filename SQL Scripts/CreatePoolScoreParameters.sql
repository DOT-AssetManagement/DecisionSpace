USE [DSS]
GO
/****** Object:  StoredProcedure [dbo].[CreateTblPoolScoreParameters]    Script Date: 5/27/2024 6:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Alter PROCEDURE [dbo].[CreateTblPoolScoreParameters] 
	 @PoolId UNIQUEIDENTIFIER,
	 @AdminPoolId UNIQUEIDENTIFIER = NULL,
	 @RowCount INT = 0 OUT
AS
BEGIN
	 SET NOCOUNT ON;

	 PRINT 'CreateTblPoolScoreParameters - started...'

	 SET @AdminPoolId =  (SELECT Id From TblUserPools WHERE Name = 'admin pool');

	 INSERT INTO TblPoolScoreParameters ([PoolId]
	  ,[Measure]
      ,[Min]
      ,[ScoreAtMin]
      ,[Max]
      ,[ScoreAtMax]
      ,[Slope]
      ,[SafetyWeight]
      ,[MobilityWeight]
      ,[EquityWeight]
      ,[EnvWeight]
      ,[CondWeight]
      ,[CreatedBy]
      ,[CreatedAt])
	   
	  SELECT @PoolId
	  ,[Measure]
      ,[Min]
      ,[ScoreAtMin]
      ,[Max]
      ,[ScoreAtMax]
      ,[Slope]
      ,[SafetyWeight]
      ,[MobilityWeight]
      ,[EquityWeight]
      ,[EnvWeight]
      ,[CondWeight]
      ,[CreatedBy]
      ,[CreatedAt]
	  FROM TblPoolScoreParameters WHERE PoolId = @AdminPoolId

	 SET @RowCount = @@ROWCOUNT;
	 PRINT 'Records inserted into TblUserPools: ' + CONVERT(VARCHAR(10),@RowCount);

	 PRINT 'CreateTblPoolScoreParameters - ended.'
END
