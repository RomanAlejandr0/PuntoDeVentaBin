﻿using PuntoDeVentaBin.Shared.Identidades;

namespace PuntoDeVentaBin.Shared.Identidades.DTOs
{
    public class FechaInicioFechaFin
    {
        public long EmpresaId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<Venta> Ventas { get; set; }
    }
}
