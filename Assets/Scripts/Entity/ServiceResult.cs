using System.Collections.Generic;

public class ServiceResult <T>
{
    public string msg;
    public List<T> data;
    public int code;
}

public class UserData
{
    public string id;
    public string username;
    public string password;
    public string nickname;
    public string token;
}