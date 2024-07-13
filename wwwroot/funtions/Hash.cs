using BCrypt.Net;
using System.Runtime.Intrinsics.Arm;

namespace Proyecto_TiendaElectronica.wwwroot.funtions
{
    public class Hash
    {
        public static string HashPassword(string password) {
            //Generar un hash seguro de la contraseña
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            // Verificar la contraseña ingresada contra el hash almacenado
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }
    }
}
