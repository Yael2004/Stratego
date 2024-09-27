using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoDataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Intentando conectarse a la base de datos...");
                using (var context = new StrategoEntities())
                {
                    var players = context.Player.ToList();

                    Console.WriteLine("Conexión exitosa a la base de datos.");
                    Console.WriteLine("Número de jugadores en la base de datos: " + players.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al conectar a la base de datos: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
            }

            Console.WriteLine("Presiona cualquier tecla para cerrar...");
            Console.ReadLine();
        }
    }

}
