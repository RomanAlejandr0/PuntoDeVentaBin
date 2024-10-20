﻿using System.ComponentModel.DataAnnotations;

namespace PuntoDeVentaBin.Shared.Identidades
{
    public class Cliente
    {
        [Key]
        public long Id { get; set; }
        public long EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

        public List<Venta> Ventas { get; set; }

    }
}
