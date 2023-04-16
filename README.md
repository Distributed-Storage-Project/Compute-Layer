# Compute-Layer
Receive Kusto query (post body) from HomeController, convert it to SQL, using https://github.com/Distributed-Storage-Project/Kusto2Sql-Belong-to-computer-layer-in-the-future- , make HTTP post query and send this query to Storage /query API.

Public API /query (Kusto query) -> Kusto-to-SQL (Compute layer) -> /api/query (Storage layer)
