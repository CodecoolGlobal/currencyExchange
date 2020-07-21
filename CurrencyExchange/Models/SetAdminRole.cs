using CurrencyExchange.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Models
{
    public static class SetAdminRole
    {
        public static void Initialize(IServiceProvider serviceProvider, int id)
        {
            using (var context = new CurrencyExchangeContext(
                    serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                var user = context.Users.First(u => u.ID == id);
                user.Role = "Admin";
                context.Update(user);
                context.SaveChanges();
            }
        }
    }
}
