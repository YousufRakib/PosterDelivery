SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_alterdiagram]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	PRINT 'DROP PROCEDURE [dbo].[sp_alterdiagram]'
	DROP PROCEDURE [dbo].[sp_alterdiagram]
END
GO

PRINT 'CREATE PROCEDURE dbo.sp_alterdiagram'
GO


CREATE PROCEDURE [dbo].[sp_alterdiagram]
(
	@diagramname    sysname,
	@owner_id       int	= null,
	@version        int,
	@definition     varbinary(max)
)
WITH EXECUTE AS 'dbo'
AS
/****************************************************************************
FILE NAME:      sp_alterdiagram
DESCRIPTION:    Alters a diagram
AUTHOR:         
*****************************************************************************

*****************************************************************************
HISTORY

Date        Initials    Description
----------  ---------   ----------------------------------------------------
01/17/23     MDK         Scripted from existing sproc
****************************************************************************/
BEGIN
	set nocount on
	
	declare @theId 			int
	declare @retval 		int
	declare @IsDbo 			int
		
	declare @UIDFound 		int
	declare @DiagId			int
	declare @ShouldChangeUID	int
	
	if(@diagramname is null)
	begin
		RAISERROR ('Invalid ARG', 16, 1)
		return -1
	end
	
	execute as caller;
	select @theId = DATABASE_PRINCIPAL_ID();	 
	select @IsDbo = IS_MEMBER(N'db_owner'); 
	if(@owner_id is null)
		select @owner_id = @theId;
	revert;
	
	select @ShouldChangeUID = 0
	select @DiagId = diagram_id, @UIDFound = principal_id from dbo.sysdiagrams where principal_id = @owner_id and name = @diagramname 
		
	if(@DiagId IS NULL or (@IsDbo = 0 and @theId <> @UIDFound))
	begin
		RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1);
		return -3
	end
	
	if(@IsDbo <> 0)
	begin
		if(@UIDFound is null or USER_NAME(@UIDFound) is null) -- invalid principal_id
		begin
			select @ShouldChangeUID = 1 ;
		end
	end

	-- update dds data			
	update dbo.sysdiagrams set definition = @definition where diagram_id = @DiagId ;

	-- change owner
	if(@ShouldChangeUID = 1)
		update dbo.sysdiagrams set principal_id = @theId where diagram_id = @DiagId ;

	-- update dds version
	if(@version is not null)
		update dbo.sysdiagrams set version = @version where diagram_id = @DiagId ;

	return 0
END
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

PRINT 'GRANT  EXECUTE  ON [dbo].[sp_alterdiagram] TO [public]'
GO

GRANT EXECUTE ON [dbo].[sp_alterdiagram] TO [public]
GO


