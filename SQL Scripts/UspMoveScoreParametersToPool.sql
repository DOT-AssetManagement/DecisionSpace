USE [DSS]
GO
/****** Object:  StoredProcedure [dbo].[UspMoveScoreParametersToPool]    Script Date: 7/5/2024 3:10:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER PROCEDURE [dbo].[UspMoveScoreParametersToPool]
	@PoolId UNIQUEIDENTIFIER,
	@keepUserCreated bit,
	@fromScratch bit
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @RowCount INT;
 
    PRINT 'dbo.UspMoveScoreParametersToPool [' + CONVERT(VARCHAR(50),@PoolId) + '] - started...';
	IF (@fromScratch = 1) BEGIN
	  IF (@keepUserCreated = 1)
		BEGIN
			DELETE FROM TblPoolScoreParameters 
			WHERE PoolId = @PoolId 
			AND (CreatedBy IS NULL OR CreatedBy LIKE 'dbo%');
		END
		ELSE
		BEGIN
		   DELETE FROM TblPoolScoreParameters  WHERE PoolId = @PoolId ;
		END
	END
 
	SET @RowCount = @@ROWCOUNT;
	PRINT 'Old records deleted from TblPoolScoreParameters: ' + CONVERT(VARCHAR(10),@RowCount);

        -- Insert records into TblPoolScoreParameters
        INSERT INTO TblPoolScoreParameters (PoolId, Measure, [Min], ScoreAtMin, [Max], ScoreAtMax, Slope, SafetyWeight, MobilityWeight, EquityWeight, EnvWeight, CondWeight)
        SELECT @PoolId, Measure, 
            ISNULL(CONVERT(FLOAT,[Min]), 0),
            ISNULL(CONVERT(FLOAT,ScoreAtMin),0), 
            ISNULL(CONVERT(FLOAT,[Max]),0), 
            ISNULL(CONVERT(FLOAT,[ScoreAtMax]),0),
            CASE WHEN (ISNULL(CONVERT(FLOAT,[Max],0),0) = ISNULL(CONVERT(FLOAT,[Min]),0)) THEN 0
                 ELSE (ISNULL(CONVERT(FLOAT,ScoreAtMax),0) - ISNULL(CONVERT(FLOAT,ScoreAtMin),0)) / (ISNULL(CONVERT(FLOAT,[Max]),0) - ISNULL(CONVERT(FLOAT,[Min]),0))
            END AS [Slope],
            ISNULL(CONVERT(FLOAT,[SafetyWeight]), 0),
            ISNULL(CONVERT(FLOAT,[MobilityWeight]), 0),
            ISNULL(CONVERT(FLOAT,[EquityWeight]),0), 
            ISNULL(CONVERT(FLOAT,[EnvWeight]),0), 
            ISNULL(CONVERT(FLOAT,[CondWeight]),0)
        FROM TblRawImportedScoreParameters WITH (NOLOCK);
    
	SET @RowCount = @@ROWCOUNT;
	PRINT 'New records inserted into from TblPoolScoreParameters: ' + CONVERT(VARCHAR(10),@RowCount);
 
	PRINT 'dbo.UspMoveScoreParametersToPool [' + CONVERT(VARCHAR(50),@PoolId) + '] - ended.'
END