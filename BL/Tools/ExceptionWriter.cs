using TreeTest.BL.Exceptions;
using TreeTest.Data;
using TreeTest.Dtos;

namespace TreeTest.BL.Tools;

public class ExceptionWriter
{
    public static long Save(TreeContext db, QueryInfo queryInfo, object ex)
    {
        var id = (from e in db.Events  
            orderby e.Id descending
            select e.Id).FirstOrDefault();
        long eventId = id.Equals(null) ? 1 : id+1;
        JournalEvent evnt = new JournalEvent()
        {
            EventId = eventId,
            CreatedAt = DateTime.UtcNow,
            Text = ExceptionInfoFormatter.GetPlainText(ex, queryInfo, eventId)
        };

        db.Events.Add(evnt);
        db.SaveChanges();

        return evnt.Id;
    }
}