using Microsoft.EntityFrameworkCore.ChangeTracking;
using Packt.Shared;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindService.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        // Use a static thread-safe dictionary field  cache the customers
        private static ConcurrentDictionary<string, Customer> customersCache;

        // Use an instance data context field because it should not be cached due to their internal caching
        private Northwind db;

        public CustomerRepository(Northwind db)
        {
            this.db = db;

            // Pre-load customers from the database as a normal Dictionary with customerID as the key,
            // then convert to a thread-safe ConcurrentDictionary
            if (customersCache == null)
            {
                customersCache = new ConcurrentDictionary<string, Customer>(
                    db.Customers.ToDictionary(c => c.CustomerID));
            }
        }

        public async Task<Customer> CreateAsync(Customer c)
        {
            // Normailze CustomerID into uppercase
            c.CustomerID = c.CustomerID.ToUpper();

            // Add to database into uppercase
            EntityEntry<Customer> added = await db.Customers.AddAsync(c);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                // If the customer is new, add her to cache, else call UpdateCache
                return customersCache.AddOrUpdate(c.CustomerID, c, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<Customer>> RetrieveAllAsync()
        {
            // For performance, fetch from cache
            return Task.Run<IEnumerable<Customer>>(
                () => customersCache.Values);
        }

        public Task<Customer> RetrieveAsync(string id)
        {
            return Task.Run(() => 
            {
                // For performance, fetch from cache
                id = id.ToUpper();
                Customer c;
                customersCache.TryGetValue(id, out c);
                return c;
            });
        }

        private Customer UpdateCache(string id, Customer c)
        {
            Customer old;
            if (customersCache.TryGetValue(id, out old))
            {
                if (customersCache.TryUpdate(id, c, old))
                {
                    return c;
                }
            }
            return null;
        }

        public async Task<Customer> UpdateAsync(string id, Customer c)
        {
            // Normalize customer ID
            id = id.ToUpper();
            c.CustomerID = c.CustomerID.ToUpper();

            // Update database
            db.Customers.Update(c);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                // Update in cache
                return UpdateCache(id, c);
            }

            return null;
        }

        public async Task<bool?> DeleteAsync(string id)
        {
            id = id.ToUpper();

            // Remove from database
            Customer c = db.Customers.Find(id);
            db.Customers.Remove(c);
            int affected = await db.SaveChangesAsync();

            if (affected == 1)
            {
                return customersCache.TryRemove(id, out c);
            }
            else
            {
                return null;
            }
        }
    }
}