using System;

namespace Test1.DTO
{
	public partial class BookDto
	{
		public int BookId { get; set; }
		public string Article { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public Nullable<System.DateTime> PublicationDate { get; set; }
		public Nullable<int> numberOfCopies { get; set; }

		public CustomerDto LastCustomer { get; set; }

		public BookDto()
		{

		}

	}
}
