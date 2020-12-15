CREATE PROCEDURE [dbo].[sp_Insert] 
    @message nvarchar(max),
    @time datetime
AS

insert into [LogsSample]( [Mesage] , [Date1Time] )
values(@message, @time)
