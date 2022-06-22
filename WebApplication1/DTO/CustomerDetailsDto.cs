using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test1.DTO
{
	public partial class CustomerDetailsDto : CustomerDto
	{
		public BookDto[] Books { get; set; }
		public CustomerDetailsDto()
		{
		}
	}
}