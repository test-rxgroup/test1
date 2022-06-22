using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Test1.DTO;
using WebApplication1.Models;

namespace WebApplication2
{
    public class BooksController : ApiController
    {
        private test1Entities db = new test1Entities();

        public IEnumerable<BookDto> Get()
        {
            return db.Books.Include(b => b.Customer).Where(b => b.DeleteAt == null).AsEnumerable().Select(a => a.GetDto());
        }

        public IEnumerable<BookDto> GetGiven()
        {
            return db.Books
                .Include(b => b.Customer)
                .Where(b => b.DeleteAt == null)
                .Where(b => b.CurrentOwn != null)
                .AsEnumerable().Select(a => a.GetDto());
        }

        public IEnumerable<BookDto> GetAvailable()
        {
            return db.Books

                .Where(b => b.DeleteAt == null)
               .Where(b => b.CurrentOwn == null)
                .AsEnumerable().Select(a => a.GetDto());
        }

        [ResponseType(typeof(BookDto))]
        [HttpGet]        
        public IHttpActionResult Get(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            if (book.DeleteAt != null)
            {
                return Conflict();
            }

            return Ok(book.GetDto());
        }


        [ResponseType(typeof(BookDto[]))]
        [HttpGet]
        public IHttpActionResult Search(string term)
        {
            var books = db.Books
                .Where(b => b.Title.Contains(term))
                .OrderBy(b => b.Article)
                .Take(10)
                .AsEnumerable();

            return Ok(books.Select(a => a.GetDto()).ToArray());
        }

        [HttpPost]
        [ResponseType(typeof(BookDto))]
        public IHttpActionResult Update([FromBody] BookDto book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbbook = db.Books.Find(book.BookId);

            if (dbbook == null) return NotFound();
            if (dbbook.DeleteAt != null) return Conflict();

            var retbook = dbbook.GetDto();

            dbbook.Title = book.Title;
            dbbook.Article = book.Article;
            dbbook.Author = book.Author;
            dbbook.numberOfCopies = book.numberOfCopies;  
            dbbook.PublicationDate = book.PublicationDate;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.BookId))
                {
                    
                }
                else
                {
                    throw;
                }
            }

            return Ok(retbook);
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        [HttpPost]
        public IHttpActionResult Create(BookDto book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newBook = db.Books.Add(new Book()
            {
                Article = book.Author,
                Author = book.Author,
                numberOfCopies = book.numberOfCopies,
                PublicationDate = book.PublicationDate,
                Title = book.Title,
                CreateAt = DateTime.UtcNow
            });
            db.SaveChanges();

            var retBook = newBook.GetDto();

            return CreatedAtRoute("DefaultApi", new { id = retBook.BookId }, retBook);
        }

        [HttpDelete]
        [HttpPost]
        [ResponseType(typeof(BookDto))]
        public IHttpActionResult Delete(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            var retbook = book.GetDto();

            book.DeleteAt = DateTime.UtcNow;
            db.SaveChanges();

            return Ok(retbook);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.BookId == id) > 0;
        }
    }
}