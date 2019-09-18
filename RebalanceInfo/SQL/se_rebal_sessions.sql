if exists (select * from sysobjects where name = 'se_rebal_sessions')
begin
	drop procedure se_rebal_sessions
	print 'PROCEDURE: se_rebal_sessions dropped'
end
go


create procedure  [dbo].[se_rebal_sessions] --se_rebal_sessions 4
(
 @session_id numeric(10) = -1
)


as
    
	select 
	CONVERT(varchar, rebal_session_id)+ ' - ' + model_name + ' - ' + CONVERT(varchar, created_time) as session,
	* from rebal_session
	where (@session_id = -1 or @session_id = rebal_session_id)

go
if @@error = 0 print 'PROCEDURE: se_rebal_sessions created'
else print 'PROCEDURE: se_rebal_sessions error on creation'
go