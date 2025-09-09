namespace App.Models;
using App.Services;
public class User : IIdentifiable
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool IsAdmin { get; set; } = false;
    public bool IsWarned { get; set; } = false;
    public User(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }
    public User() { }
    public override string ToString()
    {
        return $"User(Id: {Id}, Username: {Username}, Email: {Email}, IsAdmin: {IsAdmin}, IsWarned: {IsWarned})";
    }
    
}