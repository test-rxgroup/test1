using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test1.DTO;


namespace WebApplication1.Models
{
	public static class ModelExtension
	{
		//public 
	}

	public partial class Book
	{
		public BookDto GetDto() => (BookDto)this;
	}

	public partial class Customer
	{
		public CustomerDto GetDto() => (CustomerDto)this;
		public CustomerDetailsDto GetDetailsDto() => (CustomerDetailsDto)this;
	}
	public partial class RecordOwn
	{
		public RecordOwnDto GetDto() => (RecordOwnDto)this;

	}
}

namespace Test1.DTO
{
	using WebApplication1.Models;
	public partial class RecordOwnDto
	{
		public RecordOwnDto(RecordOwn record)
		{
			DeliveryDate = record.DeliveryDate;
			CustomerId = record.CustomerId;
			BookId = record.BookId;
			ReturnDate = record.ReturnDate;
		}
		public static implicit operator RecordOwnDto(RecordOwn x)
		{
			return new RecordOwnDto(x);
		}
	}

	public partial class BookDto
	{
		public BookDto(Book book)
		{
			Title = book.Title;
			Article = book.Article;
			Author = book.Author;
			BookId = book.BookId;
			numberOfCopies = book.numberOfCopies;
			PublicationDate = book.PublicationDate;
			LastCustomer = book.Customer?.GetDto();
		}
		public static implicit operator BookDto(Book x)
		{
			return new BookDto(x);
		}
	}

	public partial class CustomerDto
	{
		public CustomerDto(Customer customer)
		{
			Birthdate = customer.Birthdate;
			FIO = customer.FIO;
			CustomerId = customer.CustomerId;
		}
		public static implicit operator CustomerDto(Customer x)
		{
			return new CustomerDto(x);
		}
	}

	public partial class CustomerDetailsDto
	{
		public CustomerDetailsDto(Customer customer)
			: base(customer)
		{
			Books = customer.Books.Select(x => new BookDto(x)).ToArray();
		}
		public static implicit operator CustomerDetailsDto(Customer x)
		{
			return new CustomerDetailsDto(x);
		}
	}
}

