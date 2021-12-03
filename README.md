# app-tools

_Librería de utilerias generales para un proyecto realizado en dotNet_

## Comenzando 🚀

_Estas instrucciones te permitirán usar las diferentes utilerias aqui realizadas._

### Tabla de contenido 
[File_csv](#File_csv)  
<a name="File_csv"/>

## File_csv
_Utilería de importación y conversión de archivos .csv_

### Modo de uso 📋
_Generar la instancia del archivo que deseamos importar en memoria_
```
File_csv file = new File_csv(path: "C:\\archivo.csv", columnStart: 1, rowStart: 1);
```

#### Configuración de parametros 🔧
> **path:** Ruta completa del archivo.

> **columnStart:** Número de columna desde la cual se comenzará a leer el archivo.

> **rowStart:** Número de fila desde la cual se comenzará a leer el archivo.

#### Conversión a objetos 🚀
1. Generamos la relación [columna - propiedad]

```
Dictionary<int, string> properties = new Dictionary<int, string>
{
    /// La columna[0] del archivo, corresponde con la propiedad <Nombre> del objeto <Persona> que vamos a generar.
    { 0, "Nombre" },
    /// La columna[1] del archivo, corresponde con la propiedad <Apellidos> del objeto <Persona> que vamos a generar.
    { 1, "Apellidos" },
};
```
> **Nota:** Hacer esto por cada columna del archivo que querramos vaciar en una propiedad del objeto.

2. Convertimos el archivo a una lista de objetos
```
IEnumerable<Persona> vector vector = file.ToListOf<Persona>(properties);
```
> **Nota:** Si no se pueden convertir las filas del archivo en objetos, se devuelve una lista vacia.

---
Por [thejoemr](https://github.com/thejoemr)
