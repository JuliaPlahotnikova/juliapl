CREATE PROCEDURE [dbo].[sp_GetLogs]
as 
    select [Mesage], [Date1Time] from [LogsSample]
