SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_creatediagram]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	PRINT 'DROP PROCEDURE [dbo].[sp_creatediagram]'
	DROP PROCEDURE [dbo].[sp_creatediagram]
END
GO

PRINT 'CREATE PROCEDURE dbo.sp_creatediagram'
GO

CREATE PROCEDURE [dbo].[sp_creatediagram]
(
	@diagramname 	sysname,
	@owner_id		int	= null, 	
	@version 		int,
	@definition 	varbinary(max)
)
WITH EXECUTE AS 'dbo'
AS
/****************************************************************************
FILE NAME:      sp_creatediagram
DESCRIPTION:    Creates a diagram
AUTHOR:         
*****************************************************************************
COMMENTS This was converted from a static query defined in PosterDelivery.Utility.DapperQuery.cs

*****************************************************************************
HISTORY

Date        Initials    Description
----------  ---------   ----------------------------------------------------
01/17/23     MDK         Scripted from existing sproc
****************************************************************************/
BEGIN
	set nocount on
	
	declare @theId int
	declare @retval int
	declare @IsDbo	int
	declare @userName sysname
	if(@version is null or @diagramname is null)
	begin
		RAISERROR (N'E_INVALIDARG', 16, 1);
		return -1
	end
	
	execute as caller;
	select @theId = DATABASE_PRINCIPAL_ID(); 
	select @IsDbo = IS_MEMBER(N'db_owner');
	revert; 
		
	if @owner_id is null
	begin
		select @owner_id = @theId;
	end
	else
	begin
		if @theId <> @owner_id
		begin
			if @IsDbo = 0
			begin
				RAISERROR (N'E_INVALIDARG', 16, 1);
				return -1
			end
			select @theId = @owner_id
		end
	end
	-- next 2 line only for test, will be removed after define name unique
	if EXISTS(select diagram_id from dbo.sysdiagrams where principal_id = @theId and name = @diagramname)
	begin
		RAISERROR ('The name is already used.', 16, 1);
		return -2
	end
	
	insert into dbo.sysdiagrams(name, principal_id , version, definition)
			VALUES(@diagramname, @theId, @version, @definition) ;
		
	select @retval = @@IDENTITY 
	return @retval
END
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

PRINT 'GRANT  EXECUTE  ON [dbo].[sp_creatediagram] TO [public]'
GO

GRANT EXECUTE ON [dbo].[sp_creatediagram] TO [public]
GO
