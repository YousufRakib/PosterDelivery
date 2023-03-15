SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_helpdiagramdefinition]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	PRINT 'DROP PROCEDURE [dbo].[sp_helpdiagramdefinition]'
	DROP PROCEDURE [dbo].[sp_helpdiagramdefinition]
END
GO

PRINT 'CREATE PROCEDURE dbo.sp_helpdiagramdefinition'
GO

CREATE PROCEDURE [dbo].[sp_helpdiagramdefinition]
(
	@diagramname 	sysname,
	@owner_id	int	= null 		
)
WITH EXECUTE AS N'dbo'
AS
/****************************************************************************
FILE NAME:      sp_helpdiagramdefinition
DESCRIPTION:    Helps a diagram definition
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

	declare @theId 		int
	declare @IsDbo 		int
	declare @DiagId		int
	declare @UIDFound	int
	
	if(@diagramname is null)
	begin
		RAISERROR (N'E_INVALIDARG', 16, 1);
		return -1
	end
	
	execute as caller;
	select @theId = DATABASE_PRINCIPAL_ID();
	select @IsDbo = IS_MEMBER(N'db_owner');
	if(@owner_id is null)
		select @owner_id = @theId;
	revert; 
	
	select @DiagId = diagram_id, @UIDFound = principal_id from dbo.sysdiagrams where principal_id = @owner_id and name = @diagramname;
	if(@DiagId IS NULL or (@IsDbo = 0 and @UIDFound <> @theId ))
	begin
		RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1);
		return -3
	end

	select version, definition FROM dbo.sysdiagrams where diagram_id = @DiagId ; 
	return 0
END
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

PRINT 'GRANT  EXECUTE  ON [dbo].[sp_helpdiagramdefinition] TO [public]'
GO

GRANT EXECUTE ON [dbo].[sp_helpdiagramdefinition] TO [public]
GO
