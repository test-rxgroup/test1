using System;
using System.Collections.Generic;
using System.Text;

namespace Test1.DTO
{
	public partial class CustomerDto
	{
		public int CustomerId { get; set; }
		public string FIO { get; set; }
		public Nullable<System.DateTime> Birthdate { get; set; }
		public CustomerDto()
		{

		}
	}
}
