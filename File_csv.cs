using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace app_tools
{
    /// <summary>
    /// Instancia de un archivo .csv
    /// </summary>
    public class File_csv
    {
        /// <summary>
        /// Ruta del archivo leido
        /// </summary>
        public string FilePath { get; }
        /// <summary>
        /// Número de columna desde la cual se comenzó a leer el archivo
        /// </summary>
        public int ColumnStart { get; }
        /// <summary>
        /// Número de fila desde la cual se comenzó a leer el archivo
        /// </summary>
        public int RowStart { get; }
        /// <summary>
        /// Datos crudos del archivo
        /// </summary>
        public IEnumerable<string[]> FileData { get; }

        /// <summary>
        /// Crea una instancia del archivo .csv solicitado
        /// </summary>
        /// <param name="path">Ruta completa del archivo</param>
        /// <param name="columnStart">Número de columna desde la cual se comenzará a leer el archivo</param>
        /// <param name="rowStart">Número de fila desde la cual se comenzará a leer el archivo</param>
        /// <remarks><strong>Nota:</strong> Si el archivo no tiene datos o no es un archivo válido, se generará una <strong>excepcion</strong></remarks>
        public File_csv(string path, int columnStart = 0, int rowStart = 0)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' no puede ser nulo ni estar vacío.", nameof(path));
            }
            else if (!File.Exists(path))
            {
                throw new NullReferenceException($"No se encontró el archivo '{nameof(path)}'.");
            }
            else if (!Path.GetExtension(path).Equals(".csv"))
            {
                throw new NullReferenceException($"'{Path.GetFileName(path)}' no es un archivo '.csv'.");
            }

            FilePath = path;
            ColumnStart = columnStart;
            RowStart = rowStart;
            FileData = ReadFile(columnStart, rowStart);
        }

        IEnumerable<string[]> ReadFile(int startX, int startY)
        {
            try
            {
                List<string[]> rows = new();

                string[] lines = File.ReadAllLines(FilePath);
                for (int y = 0; y < lines.Length; y++)
                {
                    if (y >= startY)
                    {
                        List<string> row = new();

                        string[] line = lines[y].Split(',');
                        for (int x = 0; x < line.Length; x++)
                        {
                            if (x >= startX)
                            {
                                row.Add(line[x]);
                            }
                        }

                        // Si no es un fila vacia, la agregamos a los datos
                        if (row.Count(cell => string.IsNullOrEmpty(cell)) < row.Count)
                            rows.Add(row.ToArray());
                    }
                }

                // Si no se leyeron datos, el archivo estaba vacio
                if (!rows.Any())
                {
                    throw new Exception($"Sin datos registrados");
                }

                return rows;
            }
            catch (Exception e)
            {
                throw new Exception($"'{nameof(FilePath)}' error al leer archivo => Error: '{e.Message}'.");
            }
        }

        static bool IsConvertible(string from, Type to, out object output)
        {
            output = default;
            try
            {
                output = Convert.ChangeType(from, to);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Convierte los datos del archivo en una colección de objetos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="props">Relacion [columna - propiedad] del archivo y los objetos</param>
        /// <remarks><strong>Nota:</strong> Si no se pueden convertir las filas del archivo en objetos, se devuelve una lista vacia.</remarks>
        public IEnumerable<T> ToListOf<T>(IReadOnlyDictionary<int, string> props) where T : new()
        {
            try
            {
                List<T> objects = new();

                foreach (string[] line in FileData)
                {
                    // Generamos un nuevo objeto del tipo <T> solicitado
                    T obj = Activator.CreateInstance<T>();

                    bool isValid = false;
                    for (int i = 0; i < line.Length; i++)
                    {
                        // Si no se solicitó una propiedad que corresponda con la columna[i], omitimos la columna
                        if (!props.ContainsKey(i))
                            continue;

                        PropertyInfo objProp = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals(props[i]));

                        // Si el objeto que generamos NO TIENE una propiedad llamada como se especificó, omitimos la columna
                        if (objProp is null)
                            continue;

                        // Si el tipo de dato de la propiedad no es compatible con el dato del archivo, omitimos la columna 
                        if (!IsConvertible(from: line[i], to: objProp.PropertyType, out object value))
                            continue;

                        // Marcamos el objeto como válido y asignamos la propiedad con el valor del archivo 
                        isValid = true;
                        objProp.SetValue(obj, value);
                    }

                    /// Si la fila es válida, quiere decir que ALMENOS UNA PROPIEDAD fué asignada
                    /// Agregamos el objeto generado a la lista
                    if (isValid)
                        objects.Add(obj);
                }

                return objects;
            }
            catch (Exception e)
            {
                throw new Exception($"'{nameof(FilePath)}' error al convertir datos => Error: '{e.Message}'.");
            }
        }
    }
}
