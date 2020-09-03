using CurrencyExchange.Data;
using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CurrencyExchange.Tools
{
    public class SQLTools
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static User GetUserById(int id)
        {
            using (var context = new CurrencyExchangeContext(
                    _serviceProvider.GetRequiredService<
                        DbContextOptions<CurrencyExchangeContext>>()))
            {
                User user = context.Users.Where(userToRead => userToRead.ID == id).First();
                return user;
            }
        }
    }
}
