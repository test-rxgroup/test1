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
    public class CustomersController : ApiController
    {
        private test1Entities db = new test1Entities();

        public IEnumerable<CustomerDto> Get()
        {
            return db.Customers.Where(b => b.DeleteAt == null).AsEnumerable().Select(a => a.GetDto());
        }

        [ResponseType(typeof(CustomerDetailsDto[]))]
        [HttpGet]
        public IHttpActionResult Search(string term)
        {
            var books = db.Customers
                .Where(b => b.FIO.Contains(term))
                .OrderBy(b => b.FIO)
                .Take(10)
                .AsEnumerable();

            return Ok(books.Select(a => a.GetDetailsDto()).ToArray());
        }

        [ResponseType(typeof(CustomerDto))]
        [HttpGet]        
        public IHttpActionResult Get(int id)
        {
            Customer Customer = db.Customers.Find(id);
            if (Customer == null)
            {
                return NotFound();
            }

            return Ok(Customer.GetDto());
        }

        [ResponseType(typeof(CustomerDetailsDto))]
        [HttpGet]
        public IHttpActionResult GetDetails(int id)
        {
            Customer Customer = db.Customers.Find(id);
            if (Customer == null)
            {
                return NotFound();
            }

            return Ok(Customer.GetDetailsDto());
        }



        [HttpPost]
        [ResponseType(typeof(CustomerDto))]
        public IHttpActionResult Update([FromBody] CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbCustomer = db.Customers.Find(customer.CustomerId);

            if (dbCustomer == null) return NotFound();
            if (dbCustomer.DeleteAt != null) return Conflict();

            var retCustomer = dbCustomer.GetDto();

            dbCustomer.Birthdate = customer.Birthdate;
            dbCustomer.FIO = customer.FIO;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.CustomerId))
                {

                }
                else
                {
                    throw;
                }
            }

            return Ok(retCustomer);
        }

        // POST: api/Customers
        [ResponseType(typeof(Customer))]
        [HttpPost]
        public IHttpActionResult Create(CustomerDto customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newCustomer = db.Customers.Add(new Customer()
            {
                Birthdate = customer.Birthdate,
                FIO = customer.FIO,
                CreateAt = DateTime.UtcNow,
            });
            db.SaveChanges();

            var retCustomer = newCustomer.GetDto();

            return CreatedAtRoute("DefaultApi", new { id = retCustomer.CustomerId }, retCustomer);
        }

        // DELETE: api/Customers/5
        [HttpDelete]
        [HttpPost]
        [ResponseType(typeof(CustomerDto))]
        public IHttpActionResult Delete(int id)
        {
            Customer Customer = db.Customers.Find(id);
            if (Customer == null)
            {
                return NotFound();
            }
            var retCustomer = Customer.GetDto();

            Customer.DeleteAt = DateTime.UtcNow;
            db.SaveChanges();

            return Ok(retCustomer);
        }

        [HttpPost]
        [ResponseType(typeof(RecordOwnDto))]
        public IHttpActionResult Give(RecordOwnDto record)
        {
            var customer = db.Customers.Find(record.CustomerId);
            if (customer == null)
            {
                return NotFound();
            }
            if (customer.DeleteAt != null)
            {
                return Conflict();
            }

            var book = db.Books.Find(record.BookId);
            if (book == null)
            {
                return NotFound();
            }
            if (book.DeleteAt != null)
            {
                return Conflict();
            }
            if (book.Customer != null)
            {
                return Conflict();
            }
            if (db.RecordOwns
                .Where(r => r.BookId == record.BookId)
                .Where(r => r.ReturnDate == null)
                .Any())
                return Conflict();

            var newrecord = db.RecordOwns.Add(new RecordOwn()
            {
                BookId = record.BookId,
                CustomerId = record.CustomerId,
                DeliveryDate = DateTime.UtcNow,
            });

            book.Customer = customer;

            var retRecord = newrecord.GetDto();

            db.SaveChanges();

            return Ok(retRecord);
        }


        [HttpPost]
        [ResponseType(typeof(RecordOwnDto))]
        public IHttpActionResult Submit(RecordOwnDto record)
        {
            var customer = db.Customers.Find(record.CustomerId);
            if (customer == null)
            {
                return NotFound();
            }
            if (customer.DeleteAt != null)
            {
                return Conflict();
            }

            var book = db.Books.Find(record.BookId);
            if (book == null)
            {
                return NotFound();
            }
            if (book.DeleteAt != null)
            {
                return Conflict();
            }

            if (book.Customer == null)
            {
                return Conflict();
            }
            var dbrecord = db.RecordOwns
                   .Where(r => r.BookId == record.BookId)
                   .Where(r => r.CustomerId == record.CustomerId)
                   .Where(r => r.ReturnDate == null)
                   .First();


            if (db.RecordOwns
                .Where(r => r.BookId == record.BookId)
                .Where(r => r.ReturnDate == null)
                .Any())
                return Conflict();

            book.CurrentOwn = null;
            dbrecord.ReturnDate = DateTime.UtcNow;

            var retRecord = dbrecord.GetDto();

            db.SaveChanges();

            return Ok(retRecord);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}