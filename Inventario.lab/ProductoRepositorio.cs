using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Inventario.lab
{
    public class ProductoRepositorio
    {
        private const string CadenaConexion =
            @"Server=(localdb)\MSSQLLocalDB;" +
            @"Database=InventarioDB;" +
            @"Trusted_Connection=True;" +
            @"TrustServerCertificate=True;";

        public void Insertar(Producto p)
        {
            string sql = "INSERT INTO Productos (Nombre, Precio, Cantidad) " +
                         "VALUES (@nombre, @precio, @cantidad)";

            using (SqlConnection conexion = new SqlConnection(CadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@nombre", p.Nombre);
                comando.Parameters.AddWithValue("@precio", p.Precio);
                comando.Parameters.AddWithValue("@cantidad", p.Cantidad);

                conexion.Open();

                int filas = comando.ExecuteNonQuery();

                Console.WriteLine(
                    filas == 1 ? "Producto insertado" : "No se insertó");
            }
        }

        public List<Producto> ListarTodos()
        {
            List<Producto> lista = new List<Producto>();

            string sql = "SELECT Id, Nombre, Precio, Cantidad " +
                         "FROM Productos ORDER BY Nombre";

            using (SqlConnection conexion = new SqlConnection(CadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new Producto
                        {
                            Id = lector.GetInt32(0),
                            Nombre = lector.GetString(1),
                            Precio = lector.GetDecimal(2),
                            Cantidad = lector.GetInt32(3)
                        });
                    }
                }
            }

            return lista;
        }
    }
}