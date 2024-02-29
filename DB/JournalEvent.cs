using System.ComponentModel.DataAnnotations.Schema;

namespace TreeTest.Data;

public class JournalEvent
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public long Id { get; set; }
    public long EventId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;
    public string Text { get; set; } = "";
}