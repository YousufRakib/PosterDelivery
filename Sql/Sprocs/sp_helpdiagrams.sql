SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_helpdiagrams]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	PRINT 'DROP PROCEDURE [dbo].[sp_helpdiagrams]'
	DROP PROCEDURE [dbo].[sp_helpdiagrams]
END
GO

PRINT 'CREATE PROCEDURE dbo.sp_helpdiagrams'
GO

CREATE PROCEDURE [dbo].[sp_helpdiagrams]
(
	@diagramname sysname = NULL,
	@owner_id int = NULL
)
WITH EXECUTE AS N'dbo'
AS
/****************************************************************************
FILE NAME:      sp_helpdiagrams
DESCRIPTION:    Helps diagrams
AUTHOR:         
*****************************************************************************

*****************************************************************************
HISTORY

Date        Initials    Description
----------  ---------   ----------------------------------------------------
01/17/23     MDK         Scripted from existing sproc
****************************************************************************/
BEGIN
	DECLARE @user sysname
	DECLARE @dboLogin bit
	EXECUTE AS CALLER;
		SET @user = USER_NAME();
		SET @dboLogin = CONVERT(bit,IS_MEMBER('db_owner'));
	REVERT;
	SELECT
		[Database] = DB_NAME(),
		[Name] = name,
		[ID] = diagram_id,
		[Owner] = USER_NAME(principal_id),
		[OwnerID] = principal_id
	FROM
		sysdiagrams
	WHERE
		(@dboLogin = 1 OR USER_NAME(principal_id) = @user) AND
		(@diagramname IS NULL OR name = @diagramname) AND
		(@owner_id IS NULL OR principal_id = @owner_id)
	ORDER BY
		4, 5, 1
END
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

PRINT 'GRANT  EXECUTE  ON [dbo].[sp_helpdiagrams] TO [public]'
GO

GRANT EXECUTE ON [dbo].[sp_helpdiagrams] TO [public]
GO
