namespace RednitDev.Models;
public class EventDate
{
    public string? DateType { get; set; }
    public DateOnly? Start { get; set; }//วันงาน(เริ่ม) 
    public DateOnly? End { get; set; } 
    public DateOnly? CloseSubmit { get; set; } //แจ้งว่าปิดการรับสมัครแล้ว

}