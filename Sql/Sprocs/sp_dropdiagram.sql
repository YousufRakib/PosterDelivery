SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_dropdiagram]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	PRINT 'DROP PROCEDURE [dbo].[sp_dropdiagram]'
	DROP PROCEDURE [dbo].[sp_dropdiagram]
END
GO

PRINT 'CREATE PROCEDURE dbo.sp_dropdiagram'
GO

CREATE PROCEDURE [dbo].[sp_dropdiagram]
(
	@diagramname 	sysname,
	@owner_id	int	= null
)
WITH EXECUTE AS 'dbo'
AS
/****************************************************************************
FILE NAME:      sp_dropdiagram
DESCRIPTION:    Drops a diagram
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
	declare @IsDbo 			int
		
	declare @UIDFound 		int
	declare @DiagId			int
	
	if(@diagramname is null)
	begin
		RAISERROR ('Invalid value', 16, 1);
		return -1
	end
	
	EXECUTE AS CALLER;
	select @theId = DATABASE_PRINCIPAL_ID();
	select @IsDbo = IS_MEMBER(N'db_owner'); 
	if(@owner_id is null)
		select @owner_id = @theId;
	REVERT; 
		
	select @DiagId = diagram_id, @UIDFound = principal_id from dbo.sysdiagrams where principal_id = @owner_id and name = @diagramname 
	if(@DiagId IS NULL or (@IsDbo = 0 and @UIDFound <> @theId))
	begin
		RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1)
		return -3
	end
	
	delete from dbo.sysdiagrams where diagram_id = @DiagId;
	
	return 0;
END
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

PRINT 'GRANT  EXECUTE  ON [dbo].[sp_dropdiagram] TO [public]'
GO

GRANT EXECUTE ON [dbo].[sp_dropdiagram] TO [public]
GO
