﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PuntoDeVentaBin.Shared;
using PuntoDeVentaBin.Shared.Identidades;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using PuntoDeVentaBin.Shared.AccesoDatos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PuntoDeVentaBin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizacionController : ControllerBase
    {
        private IConfiguration config;
        private ApplicationDbContext datos;

        public AutorizacionController(IConfiguration config, ApplicationDbContext datos)
        {
            this.config = config;
            this.datos = datos;
        }

        [HttpPost]
        public async Task<Respuesta<UserToken>> Post([FromBody] UserInfo usuario)
        {
            var respuesta = new Respuesta<UserToken>() { Datos = new UserToken() };
            try
            {
                var user = await datos.UsuariosBin.AsNoTracking()
                   .FirstOrDefaultAsync(u => u.Email == usuario.Email.Trim())
                   .ConfigureAwait(false);

                if (user == null)
                {
                    // Caso 1: Correo electrónico no registrado
                    respuesta.Estado = EstadosDeRespuesta.NoProceso;
                    respuesta.Mensaje = "El correo electrónico no está registrado.";
                }
                else if (!user.CuentaActivada)
                {
                    // Caso 2: Cuenta no activada
                    respuesta.Estado = EstadosDeRespuesta.NoProceso;
                    respuesta.Mensaje = "La cuenta no está activada. Por favor, activa tu cuenta.";
                }
                else
                {
                    if (user.Password == usuario.Password.Trim())
                    {
                        // Caso 4: Autenticación exitosa
                        respuesta.Datos = BuildToken(user);
                        respuesta.Estado = EstadosDeRespuesta.Correcto;
                    }
                    else
                    {
                        // Caso 3: Contraseña incorrecta
                        respuesta.Estado = EstadosDeRespuesta.NoProceso;
                        respuesta.Mensaje = "La contraseña es incorrecta";
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = "Error al autenticar.";
                // Opcional: Registrar el error en un sistema de logging
            }
            return respuesta;
        }

        private UserToken BuildToken(UsuarioBin usuario)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Nombre),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("AuthId", usuario.Id.ToString()),
            };

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expira = DateTime.UtcNow.AddDays(1);

            JwtSecurityToken token = new JwtSecurityToken
            (
                issuer: null,
                audience: null,
                claims: claims,
                expires: expira,
                signingCredentials: creds
            );

            return new UserToken { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = expira };
        }


        //[HttpPost]
        //public async Task<Respuesta<UserToken>> SeleccionarNegocio([FromBody] Negocio negocioInfo)
        //{
        //    var respuesta = new Respuesta<UserToken>() { Datos = new UserToken() };
        //    try
        //    {
        //        // Obtener el ID del usuario autenticado desde el token
        //        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AuthId");
        //        if (userIdClaim == null)
        //        {
        //            //respuesta.Estado = EstadosDeRespuesta.NoAutorizado;
        //            respuesta.Estado = EstadosDeRespuesta.Error;
        //            respuesta.Mensaje = "Usuario no autenticado";
        //            return respuesta;
        //        }

        //        var userId = int.Parse(userIdClaim.Value);

        //        // Obtener el usuario desde la base de datos
        //        var user = await datos.UsuariosBin.AsNoTracking()
        //            .FirstOrDefaultAsync(u => u.Id == userId)
        //            .ConfigureAwait(false);

        //        if (user != null)
        //        {
        //            // Verificar que el usuario tiene acceso al negocio seleccionado
        //            var tieneAcceso = await datos.UsuariosRolesNegocios.AsNoTracking()
        //                .AnyAsync(urn => urn.UsuarioId == userId && urn.NegocioId == negocioInfo.Id)
        //                .ConfigureAwait(false);

        //            if (!tieneAcceso)
        //            {
        //                //respuesta.Estado = EstadosDeRespuesta.NoAutorizado;
        //                respuesta.Estado = EstadosDeRespuesta.Error;
        //                respuesta.Mensaje = "No tiene acceso al negocio seleccionado";
        //                return respuesta;
        //            }

        //            // Obtener los roles y permisos del usuario para el negocio seleccionado
        //            var rolesPermisos = await (from urn in datos.UsuariosRolesNegocios
        //                                       join rp in datos.Tbl_RolesPermisos on urn.RolId equals rp.RolId
        //                                       join p in datos.Tbl_Permisos on rp.PermisoId equals p.PermisoId
        //                                       where urn.UsuarioId == userId && urn.NegocioId == negocioInfo.Id
        //                                       select p).ToListAsync().ConfigureAwait(false);

        //            // Crear nuevos claims con los permisos del negocio
        //            var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.Nombre.ToUpper().Trim()),
        //        new Claim("AuthId", user.Id.ToString()),
        //        new Claim("Id", negocioInfo.Id.ToString()),
        //        new Claim("Nombre", user.Nombre.ToString())
        //    };

        //            // Agregar los permisos como roles
        //            foreach (var permiso in rolesPermisos)
        //            {
        //                claims.Add(new Claim(ClaimTypes.Role, permiso.Descripcion));
        //            }

        //            // Generar un nuevo token JWT con los permisos del negocio
        //            respuesta.Datos = BuildToken(user, claims);
        //            respuesta.Estado = EstadosDeRespuesta.Correcto;
        //        }
        //        else
        //        {
        //            respuesta.Estado = EstadosDeRespuesta.NoProceso;
        //            respuesta.Mensaje = "Usuario no encontrado";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Mensaje = "Error al procesar la selección del negocio.";
        //    }
        //    return respuesta;
        //}



        //[HttpPost]
        //public async Task<Respuesta<UserToken>> Post([FromBody] UserInfo usuario)
        //{
        //    var respuesta = new Respuesta<UserToken>() { Datos = new UserToken() };
        //    try
        //    {
        //        var user = await datos.UsuariosBin.AsNoTracking().
        //            FirstOrDefaultAsync(u => u.Password.DesEncriptar() == usuario.Password.Trim() && u.Email == usuario.Email).
        //            ConfigureAwait(false);

        //        if (user != null)
        //        {
        //            var UserName = user.Nombre.ToLower();
        //            var permisosPerfil = await (from pt in datos.Tbl_RolesPermisos
        //                                        join t in datos.Tbl_Permisos on pt.PermisoId equals t.PermisoId
        //                                        where pt.RolId == user.RolId
        //                                        select t).ToListAsync().ConfigureAwait(false);

        //            var identity = new IdentityUser { UserName = UserName };

        //            var authProperties = new AuthenticationProperties
        //            {
        //                IsPersistent = true,
        //                ExpiresUtc = DateTime.UtcNow.AddDays(1)
        //            };

        //            authProperties.Items.Add("IdAuthUsuario", user.Id.ToString());

        //            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Nombre.ToUpper().Trim()) };

        //            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")), authProperties).ConfigureAwait(true);

        //            respuesta.Datos = BuildToken(user, permisosPerfil);
        //            respuesta.Estado = EstadosDeRespuesta.Correcto;
        //        }
        //        else
        //        {
        //            respuesta.Estado = EstadosDeRespuesta.NoProceso;
        //            respuesta.Mensaje = "Error de usuario y contraseña";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Mensaje = "Error al authenticar";
        //    }
        //    return respuesta;
        //}


        //private UserToken BuildToken(UsuarioBin usuario, List<Tbl_Permiso> permisos)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Nombre),
        //        new Claim(ClaimTypes.Name, usuario.Nombre),
        //        new Claim(ClaimTypes.Email, usuario.Email),
        //        new Claim("AuthId", usuario.Id.ToString()),
        //        new Claim("EmpresaId", usuario.NegocioId.ToString()),
        //        new Claim("Nombre", usuario.Nombre.ToString())
        //    };

        //    foreach (var permiso in permisos)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, permiso.Descripcion));
        //    }
        //    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["jwt:key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var expira = DateTime.UtcNow.AddDays(1);

        //    JwtSecurityToken token = new JwtSecurityToken
        //    (
        //        issuer: null,
        //        audience: null,
        //        claims: claims,
        //        expires: null,
        //        signingCredentials: creds
        //    );
        //    return new UserToken { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = expira };
        //}




    }
}
