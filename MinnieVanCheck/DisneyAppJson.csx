using System.Collections.Generic;

public class RootObject
{
    public List<Message> messages { get; set; }
}

public class Message
{
    public string messageId { get; set; }
    public string template { get; set; }
    public Payload payload { get; set; }
    public bool showOffline { get; set; }
    public string showRule { get; set; }
    public long endDate { get; set; }
    public int startDate { get; set; }
    public Audience[] audiences { get; set; }
    public Trigger[] triggers { get; set; }
}

public class Payload
{
    public string url { get; set; }
    public string cancel { get; set; }
    public string confirm { get; set; }
    public string content { get; set; }
    public string title { get; set; }
    public string html { get; set; }
    public object[] assets { get; set; }
}

public class Audience
{
    public string key { get; set; }
    public string matches { get; set; }
    public object[] values { get; set; }
}

public class Trigger
{
    public string key { get; set; }
    public string matches { get; set; }
    public object[] values { get; set; }
}
