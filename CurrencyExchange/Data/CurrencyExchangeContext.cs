using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Data
{
    public class CurrencyExchangeContext : DbContext
    {
        public CurrencyExchangeContext(DbContextOptions<CurrencyExchangeContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
