namespace CqrsShowCase.Domain;
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public List<UserLog> Logs { get; set; }
}