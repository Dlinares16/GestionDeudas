using GestionDeudas.DAL.Repositorios.Contrato;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeudas.DAL.Repositorios
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        private readonly DbContext _context;
        public GenericRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<TModel>> Consultar(Expression<Func<TModel, bool>> filtro = null)
        {
            try
            {
                IQueryable<TModel> queryModelo = filtro == null ? _context.Set<TModel>() : _context.Set<TModel>().Where(filtro);
                return queryModelo;
            }
            catch
            {
                throw;
            }
        }

        public async Task<TModel> Crear(TModel modelo)
        {
            try
            {
                _context.Set<TModel>().Add(modelo);
                await _context.SaveChangesAsync();
                return modelo;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(TModel modelo)
        {
            try
            {
                _context.Set<TModel>().Update(modelo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(TModel modelo)
        {
            try
            {
                _context.Set<TModel>().Remove(modelo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<TModel> Obtener(Expression<Func<TModel, bool>> filtro)
        {
            try
            {
                TModel modelo = await _context.Set<TModel>().FirstOrDefaultAsync(filtro);
                return modelo;
            }
            catch
            {
                throw;
            }
        }
    }
}
