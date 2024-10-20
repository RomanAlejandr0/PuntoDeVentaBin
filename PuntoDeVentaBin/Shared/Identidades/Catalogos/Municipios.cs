﻿using System.ComponentModel.DataAnnotations;

namespace PuntoDeVentaBin.Shared.Identidades.Catalogos
{
    public class Municipios
    {
        [Key]
        public string c_Municipio { get; set; }

        public string c_Estado { get; set; }

        public string Descripcion { get; set; }
    }
}
