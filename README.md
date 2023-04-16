# Compute-Layer

For testing: <br>
Post, url e.g. Postman: http://localhost:5000/query<br>
Input format Json, e.g. :
{
  "query": "Logs | where Timestamp >= \"2000-1-1 05:00:00\" |where level == \"p\" | limit 19"
}
<br><br>

Tasks:
1. Receive Kusto query (post body) from HomeController, convert it to SQL, using https://github.com/Distributed-Storage-Project/Kusto2Sql-Belong-to-computer-layer-in-the-future- , make HTTP post query and send this query to Storage /query API.

Public API /query (Kusto query) -> Kusto-to-SQL (Compute layer) -> /api/query (Storage layer)


 
2. Receive a response (array of rows) from /api/query (Storage layer) and return it to User.

/api/query (Storage layer) -> HTTP response with array of rows -> Compute layer (HomeController) -> User


