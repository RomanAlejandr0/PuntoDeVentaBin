﻿using System.ComponentModel.DataAnnotations;

namespace PuntoDeVentaBin.Shared.Identidades.Adm_PerfilTareas
{
    public class RolPermiso
    {
        [Key]
        public int RolID { get; set; }

        public int PermisoID { get; set; }
    }
}
