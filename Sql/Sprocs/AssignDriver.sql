SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AssignDriver]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	PRINT 'DROP PROCEDURE [dbo].[AssignDriver]'
	DROP PROCEDURE [dbo].[AssignDriver]
END
GO

PRINT 'CREATE PROCEDURE dbo.AssignDriver'
GO

CREATE PROCEDURE [dbo].[AssignDriver]
(
	@UserDriverId           NUMERIC(18, 0) = 0,
	@CustomerId             NUMERIC(18, 0) = 0,
	@ActualProductsPicked   NUMERIC(18, 0) = 0,
	@ActualProductsShipped  NUMERIC(18, 0) = 0,
	@IsCompleted            BIT            = 0,
	@ReasonForNotCompletion NVARCHAR(500)  = NULL,
	@ActualDate             DATETIME,
	@IsActive               BIT            = 0,
	@CreatedByUser          NUMERIC(18, 0) = 0
)
AS
/****************************************************************************
FILE NAME:      AssignDriver
DESCRIPTION:    Gets a Company from the Company table
AUTHOR:         Mark Kennedy
*****************************************************************************
COMMENTS This was converted from a static query defined in PosterDelivery.Utility.DapperQuery.cs

*****************************************************************************
HISTORY

Date        Initials    Description
----------  ---------   ----------------------------------------------------
01/7/23     MDK         Created
****************************************************************************/
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION

		DECLARE @DeliveryDay  INT
		DECLARE @ProposedDate DATETIME

		SELECT @DeliveryDay = DeliveryDay FROM Customers with(nolock) WHERE CustomerId = @CustomerId

		IF @DeliveryDay     = 0 OR @DeliveryDay = NULL
			BEGIN
				SET @ProposedDate = GETDATE()
			END
		ELSE
			BEGIN
				SET @ProposedDate = datefromparts(YEAR(GETDATE()), MONTH(GETDATE()), @DeliveryDay)
			END

		PRINT @ProposedDate

		INSERT INTO DriverCustomerTrack(UserDriverId, CustomerId, ActualProductsPicked, ActualProductsShipped, ProposedDate, ActualDate, IsCompleted, ReasonForNotCompletion, IsActive, DateCreated, DateModified, CreatedByUser) VALUES(@UserDriverId, @CustomerId, @ActualProductsPicked, @ActualProductsShipped, @ProposedDate,@ActualDate, @IsCompleted, @ReasonForNotCompletion, @IsActive, GETDATE(), GETDATE(), @CreatedByUser)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END
	END CATCH
END
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

PRINT 'GRANT  EXECUTE  ON [dbo].[AssignDriver] TO [public]'
GO

GRANT EXECUTE ON [dbo].[AssignDriver] TO [public]
GO

/*

--AssignDriver 4, 1, 1, 0, 1, NULL, 1, 7

EXEC [dbo].[AssignDriver] 
	@UserDriverId           = 0,
	@CustomerId             = 0,
	@ActualProductsPicked   = 0,
	@ActualProductsShipped  = 0,
	@IsCompleted            = 0,
	@ReasonForNotCompletion = NULL,
	@ActualDate             = GETDATE(),
	@IsActive               = 0,
	@CreatedByUser          = 0

*/



