using TreeTest.BL.Exceptions;
using TreeTest.Data;
using TreeTest.Dtos;

namespace TreeTest.BL.Tools;

public class ExceptionWriter
{
    public static long SaveSecure(TreeContext db, QueryInfo queryInfo, SecureException se)
    {
        var id = (from e in db.Events  
            orderby e.Id descending
            select e.Id).FirstOrDefault();
        long eventId = id.Equals(null) ? 1 : id+1;
        string errDescr = ExceptionInfoFormatter.GetPlainText(se, queryInfo, eventId);
        JournalEvent evnt = new JournalEvent()
        {
            EventId = eventId,
            CreatedAt = DateTime.UtcNow,
            Text = errDescr
        };

        db.Events.Add(evnt);
        db.SaveChanges();

        return evnt.Id;
    }

    public static long Save(TreeContext db, QueryInfo queryInfo, Exception se)
    {
        var id = (from e in db.Events  
            orderby e.Id descending
            select e.Id).FirstOrDefault();
        long eventId = id.Equals(null) ? 1 : id+1;
        JournalEvent evnt = new JournalEvent()
        {
            EventId = eventId,
            CreatedAt = DateTime.UtcNow,
            Text = $"Internal server error ID = {eventId}."
        };

        db.Events.Add(evnt);
        db.SaveChanges();

        return evnt.Id;
    }
}