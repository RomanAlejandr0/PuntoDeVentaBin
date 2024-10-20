﻿using System.ComponentModel.DataAnnotations;

namespace PuntoDeVentaBin.Shared.Identidades
{
    public class ResetPasswordModel
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,10}",
        ErrorMessage = "Formato de contraseña invalido. La contraseña debe tener de 8 a 10 caracteres. " +
            "Al menos una letra minuscula, una letra mayuscula, un numero y un caracter especial")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string NuevaContraseña { get; set; }

        [Required(ErrorMessage = "La confirmación de contraseña es requerida.")]
        [Compare("NuevaContraseña", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContraseña { get; set; }
    }
}
