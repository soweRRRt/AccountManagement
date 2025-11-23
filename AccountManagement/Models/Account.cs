using LiteDB;
using System;

namespace AccountManagement.Models;

public class Account
{
    [BsonId]
    public int Id { get; set; }

    public string Title { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string Notes { get; set; }
    public string Category { get; set; }
    public string IconPath { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public Account()
    {
        CreatedAt = DateTime.Now;
        ModifiedAt = DateTime.Now;
        IsFavorite = false;
    }
}