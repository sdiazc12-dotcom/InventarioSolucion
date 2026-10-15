using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Inventario.lab
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ruta = "inventario.csv";
            string opcion;

            List<Producto> inventario = Cargar(ruta);
            ProductoRepositorio repo = new ProductoRepositorio();

            int siguienteId = 1;

            foreach (Producto p in inventario)
            {
                if (p.Id >= siguienteId)
                {
                    siguienteId = p.Id + 1;
                }
            }

            do
            {
                opcion = LeerOpcionMenu();

                switch (opcion)
                {
                    case "1":
                        Agregar(inventario, ref siguienteId);
                        break;

                    case "2":
                        Listar(inventario);
                        break;

                    case "3":
                        BuscarProducto(inventario);
                        break;

                    case "4":
                        ListarEnSqlServer(repo);
                        break;

                    case "5":
                        AgregarEnSqlServer(repo);
                        break;

                    case "6":
                        Guardar(inventario, ruta);
                        Console.WriteLine("Saliendo del sistema...");
                        break;

                    default:
                        Console.WriteLine("Opcion no valida, intenta de nuevo.");
                        break;
                }
            }
            while (opcion != "6");
        }

        public static string LeerOpcionMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=== MENU INVENTARIO ===");
            Console.WriteLine("1. Agregar producto");
            Console.WriteLine("2. Listar productos");
            Console.WriteLine("3. Buscar producto");
            Console.WriteLine("4. Listar productos de SQL Server");
            Console.WriteLine("5. Agregar producto en SQL Server");
            Console.WriteLine("6. Salir");
            Console.Write("Elige una opcion: ");

            return Console.ReadLine() ?? "";
        }

        private static void Agregar(List<Producto> inv, ref int id)
        {
            Console.WriteLine();
            Console.Write("Nombre: ");
            string nombre = (Console.ReadLine() ?? "").Trim();

            Console.Write("Precio: ");
            bool precioOk = decimal.TryParse(
                Console.ReadLine(),
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal precio);

            Console.Write("Cantidad: ");
            bool cantidadOk = int.TryParse(
                Console.ReadLine(),
                out int cantidad);

            if (string.IsNullOrWhiteSpace(nombre) ||
                !precioOk || precio <= 0 ||
                !cantidadOk || cantidad < 0)
            {
                Console.WriteLine("Datos invalidos.");
                return;
            }

            inv.Add(new Producto
            {
                Id = id,
                Nombre = nombre,
                Precio = precio,
                Cantidad = cantidad
            });

            id++;

            Console.WriteLine("Producto agregado correctamente.");
        }

        private static void Listar(List<Producto> inv)
        {
            if (inv.Count == 0)
            {
                Console.WriteLine("Inventario vacio.");
                return;
            }

            decimal totalGeneral = 0;

            Console.WriteLine();
            Console.WriteLine(
                $"{"Id",-3} {"Nombre",-20} {"Precio",10} {"Cantidad",10} {"Valor total",12}");

            foreach (Producto p in inv)
            {
                Console.WriteLine(p);
                totalGeneral += p.ValorTotal();
            }

            Console.WriteLine($"Valor total del inventario: Q{totalGeneral:N2}");
        }

        private static void BuscarProducto(List<Producto> inv)
        {
            Console.Write("Texto a buscar: ");

            string texto = (Console.ReadLine() ?? "").Trim();

            bool encontrado = false;

            foreach (Producto p in inv)
            {
                if (p.Nombre.Contains(
                    texto,
                    StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(p);
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("Sin coincidencias.");
            }
        }

        private static void Guardar(List<Producto> inv, string ruta)
        {
            List<string> lineas = new List<string>();

            foreach (Producto p in inv)
            {
                lineas.Add(
                    $"{p.Id};{p.Nombre};{p.Precio};{p.Cantidad}");
            }

            File.WriteAllLines(ruta, lineas);

            Console.WriteLine(
                $"Inventario guardado en {ruta}. Productos: {inv.Count}");
        }

        private static List<Producto> Cargar(string ruta)
        {
            List<Producto> inv = new List<Producto>();

            if (!File.Exists(ruta))
            {
                return inv;
            }

            foreach (string linea in File.ReadLines(ruta))
            {
                string[] campos = linea.Split(';');

                if (campos.Length != 4)
                {
                    continue;
                }

                bool idOk = int.TryParse(campos[0].Trim(), out int id);
                string nombre = campos[1].Trim();

                bool precioOk = decimal.TryParse(
                    campos[2].Trim(),
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal precio);

                bool cantidadOk = int.TryParse(
                    campos[3].Trim(),
                    out int cantidad);

                if (idOk && !string.IsNullOrWhiteSpace(nombre) &&
                    precioOk && cantidadOk)
                {
                    inv.Add(new Producto
                    {
                        Id = id,
                        Nombre = nombre,
                        Precio = precio,
                        Cantidad = cantidad
                    });
                }
            }

            Console.WriteLine(
                $"Cargados {inv.Count} productos desde {ruta}.");

            return inv;
        }

        private static void AgregarEnSqlServer(ProductoRepositorio repo)
        {
            Console.WriteLine();
            Console.Write("Nombre: ");
            string nombre = (Console.ReadLine() ?? "").Trim();

            Console.Write("Precio: ");
            bool precioOk = decimal.TryParse(
                Console.ReadLine(),
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal precio);

            Console.Write("Cantidad: ");
            bool cantidadOk = int.TryParse(
                Console.ReadLine(),
                out int cantidad);

            if (string.IsNullOrWhiteSpace(nombre) ||
                !precioOk || precio <= 0 ||
                !cantidadOk || cantidad < 0)
            {
                Console.WriteLine("Datos invalidos.");
                return;
            }

            repo.Insertar(new Producto
            {
                Nombre = nombre,
                Precio = precio,
                Cantidad = cantidad
            });
        }

        private static void ListarEnSqlServer(ProductoRepositorio repo)
        {
            List<Producto> lista = repo.ListarTodos();

            if (lista.Count == 0)
            {
                Console.WriteLine("No hay productos en SQL Server.");
                return;
            }

            decimal totalGeneral = 0;

            Console.WriteLine();
            Console.WriteLine("=== PRODUCTOS EN SQL SERVER ===");

            foreach (Producto p in lista)
            {
                Console.WriteLine(p);
                totalGeneral += p.ValorTotal();
            }

            Console.WriteLine(
                $"Valor total del inventario: Q{totalGeneral:N2}");
        }
    }
}