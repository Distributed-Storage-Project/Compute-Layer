public class QueryResponseDto
{
    public bool IsSuccess { get; set; }
    public List<List<string>> Data { get; set; }
    public string Message { get; set; }
}