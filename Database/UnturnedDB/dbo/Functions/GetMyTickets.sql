CREATE FUNCTION dbo.GetMyTickets (@PlayerId VARCHAR(255))
RETURNS TABLE 
AS RETURN (

	WITH CTE AS (
		SELECT TicketId, TicketTitle, TicketContent, TicketCategory, MasterTicketAuthor = TicketAuthorId, TicketAuthorId, TargetTicketId = TicketId, TicketUpdate, TicketCreated
		FROM dbo.Tickets t
		WHERE t.TargetTicketId IS NULL 
		UNION ALL 
		SELECT r.TicketId, r.TicketTitle, r.TicketContent, r.TicketCategory, MasterTicketAuthor = m.MasterTicketAuthor, r.TicketAuthorId, r.TargetTicketId, r.TicketUpdate, r.TicketCreated
		FROM CTE m
			JOIN dbo.Tickets r ON r.TargetTicketId = m.TicketId
	)
	SELECT 
		TicketId, 
		TicketTitle, 
		TicketContent, 
		TicketCategory, 
		TicketAuthorId, 
		TargetTicketId = CASE TargetTicketId WHEN TicketId THEN NULL ELSE TargetTicketId END, 
		TicketUpdate, 
		TicketCreated,
		PlayerName,
		PlayerCountry,
		Balance,
		Role,
		PlayerLastActivity,
		PlayerCreated
	FROM CTE c1
		JOIN dbo.Players p ON p.PlayerId = c1.TicketAuthorId
	WHERE MasterTicketAuthor = @PlayerId 
	OR TicketAuthorId = @PlayerId
	OR EXISTS (SELECT * FROM dbo.Tickets t1 WHERE t1.TicketAuthorId = @PlayerId AND t1.TargetTicketId = c1.TargetTicketId)
);