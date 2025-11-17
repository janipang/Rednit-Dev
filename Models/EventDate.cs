namespace RednitDev.Models;
public class EventDate
{
    public string DateType { get; set; } = "one";
    public DateOnly Start { get; set; }
    public DateOnly? End { get; set; }
    public DateOnly CloseSubmit { get; set; }
}