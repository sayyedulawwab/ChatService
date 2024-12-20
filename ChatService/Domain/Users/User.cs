﻿namespace ChatService.Domain.Users;
public sealed class User
{
    private User(string name, string email, string passwordHash, DateTime createdOn)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedOn = createdOn;
    }

    private User()
    {
    }

    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedOn { get; private set; }

    public static User Create(string name, string email, string passwordHash, DateTime createdOn)
    {
        var user = new User(name, email, passwordHash, createdOn);

        return user;
    }
}