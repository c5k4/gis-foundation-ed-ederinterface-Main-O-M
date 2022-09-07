USE [PLDB]
GO

/****** Object:  StoredProcedure [dbo].[spsync_UpdateLatLong]    Script Date: 6/12/2018 8:50:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/****** Script for SelectTopNRows command from SSMS  ******/
CREATE PROCEDURE [dbo].[spsync_UpdateLatLong]
AS

Declare     @pldbid [bigint],
			@guid [uniqueidentifier],
			@elevation [float],
            @latitude [float], 
			@longitude [float]

Declare curP cursor For

	SELECT PLDBID, EDGIS_GLOBALID, Elevation, Latitude, Longitude
	FROM [dbo].[SYNC_LAT_LONG_UPDATES];

OPEN curP
 
Fetch Next From curP Into @pldbid, @guid, @elevation, @latitude, @longitude 

While @@Fetch_Status = 0 Begin

    print @pldbid
	print @guid
	print @elevation
    print @latitude
	print @longitude 

	UPDATE dbo.OCalcProAnalysis SET Elevation = @elevation, Latitude = @latitude, Longitude = @longitude 
	WHERE PLDBID = @pldbid;
	UPDATE dbo.OCalcProAnalysisRoot SET Elevation = @elevation, Latitude = @latitude, Longitude = @longitude
	WHERE ANALYSISID IN 
	(	
	SELECT ANALYSISID FROM OCalcProAnalysis WHERE PLDBID = @pldbid AND ANALYSISID <> '00000000-0000-0000-0000-000000000000'
	);

Fetch Next From curP Into @pldbid, @guid, @elevation, @latitude, @longitude 

End -- End of Fetch

Close curP
Deallocate curP
GO


