﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaBin.Shared;
using PuntoDeVentaBin.Shared.AccesoDatos;
using PuntoDeVentaBin.Shared.Identidades.Productos;

namespace PuntoDeVentaBin.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public CategoriasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("GetAll/{empresaId}")]
        public async Task<Respuesta<List<Categoria>>> GetAll(long empresaId)
        {
            var respuesta = new Respuesta<List<Categoria>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await context.ProductoCategorias.
                    Where(x => x.EmpresaId == empresaId).
                    OrderBy(x => x.Nombre).
                    AsNoTracking().
                    ToListAsync();
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar las categorias";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> Guardar([FromBody] Categoria categoria)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            if (categoria.Id == 0)
            {
                respuesta = await GuardarCategoria(categoria);
            }
            else
            {
                respuesta = await EditarCategoria(categoria);
            }

            return respuesta;
        }

        public async Task<Respuesta<long>> GuardarCategoria(Categoria categoria)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                context.Add(categoria);
                await context.SaveChangesAsync();
                respuesta.Datos = categoria.Id;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al guardar la categoria {categoria.Nombre}";
            }

            return respuesta;
        }

        public async Task<Respuesta<long>> EditarCategoria(Categoria categoria)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                context.Attach(categoria).State = EntityState.Modified;
                await context.SaveChangesAsync();
                respuesta.Datos = categoria.Id;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar los cambios {categoria.Nombre}";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> EliminarCategoria([FromBody] Categoria categoria)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                context.Database.ExecuteSqlInterpolated($"DELETE FROM CategoriasProductos WHERE Id = {categoria.Id}");

                context.Database.ExecuteSqlInterpolated($"UPDATE Productos SET Categoria = {null} WHERE Categoria = {categoria.Nombre}");

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al eliminar la categoria de base de datos";
            }

            return respuesta;
        }

    }
}
