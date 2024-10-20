﻿using System.ComponentModel.DataAnnotations;

namespace PuntoDeVentaBin.Shared.Identidades.Adm_PerfilTareas
{
    public class Rol
    {
        [Key]
        [Required(ErrorMessage = "El perfil es requerido")]
        public int RolId { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }

        public List<UsuarioRolNegocio> UsuariosRolesNegocios { get; set; }

    }
}
