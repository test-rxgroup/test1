using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test1.DTO
{
	public partial class RecordOwnDto
	{
		public int CustomerId { get; set; }
		public int BookId { get; set; }
		public Nullable<System.DateTime> DeliveryDate { get; set; }
		public Nullable<System.DateTime> ReturnDate { get; set; }
		public RecordOwnDto()
		{

		}
	}
}