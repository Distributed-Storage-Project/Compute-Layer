# Compute-Layer

For local testing (e.g. using Postman): <br>
Make sure storage layer is running, modify home controller to request on storage layer ("https://localhost:7076/api/query", default) <br>
<br><br><br>
For reading data:<br>
=>Post to api: http://localhost:5000/query<br>
Input format Json, e.g. :
{
  "query": "Logs | where Timestamp >= \"2000-1-1 05:00:00\" | where level == \"p\" | limit 19"
}
<br><br>
For inserting data/deleting data/creating table/dropping table: <br>
    =>First login: <br>
      Post to api http://localhost:5000/query/login<br>
      Format: {
               "Username": "string",
               "Password": "string"
               }
      <br>
If user has access to write flow (e.g. system admin), token will be returned. <br>
=>(e.g. in Postman): Headers ->    set Key as "Authorization", and Value as "Bearer \<token\>" //replace token with token returned, valid for 60 min.<br>
=>Post to api respectively, with autorization token => <br>
http://localhost:5000/query/insert<br>
http://localhost:5000/query/delete<br>
http://localhost:5000/query/create<br>
http://localhost:5000/query/drop<br>


