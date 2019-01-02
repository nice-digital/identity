using System;
using System.Collections.Generic;

namespace NICE.Identity.Models
{
    public partial class Audit
    {
	    public Audit()
	    {
	    }

	    public Audit(string tableName, string fieldName, string oldValue, string newValue, DateTime date, Guid userId)
	    {
		    TableName = tableName;
		    FieldName = fieldName;
		    OldValue = oldValue;
		    NewValue = newValue;
		    Date = date;
		    UserId = userId;
	    }

	    public int AuditId { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
    }
}
