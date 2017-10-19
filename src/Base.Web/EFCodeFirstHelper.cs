using Base.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Base.Web
{
    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<InstanceEntities>
    {
        public InstanceEntities CreateDbContext(string[] args)
        {
           
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{"Development"}.json", optional: true)
                .AddEnvironmentVariables();
                
            var configuration = builder.Build();
            var optionBuilder = new DbContextOptionsBuilder<InstanceEntities>();
            optionBuilder.UseSqlServer(configuration.GetConnectionString("BaseDBConnection"),
                b => b.MigrationsAssembly("Base.Web")
                );
            return new InstanceEntities(optionBuilder.Options);
        }
    }
}
