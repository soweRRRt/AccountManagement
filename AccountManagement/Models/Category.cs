using LiteDB;
using System;

namespace AccountManagement.Models;

public class Category
{
    [BsonId]
    public int Id { get; set; }

    public string Name { get; set; }
    public string IconPath { get; set; }
    public DateTime CreatedAt { get; set; }

    public Category()
    {
        CreatedAt = DateTime.Now;
    }
}