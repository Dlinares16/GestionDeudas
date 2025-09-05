using GestionDeudas.DAL.DBContext;
using GestionDeudas.DAL.Repositorios;
using GestionDeudas.DAL.Repositorios.Contrato;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.IOC
{
    public static class Dependencia
    {
        public static void RegistrarDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GestionDeudasContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
    }
}
