namespace csharp_ej1
{
    // Clase que contiene funciones que son compartidas entre diferentes clases
    class Utilities
    {
        // Realiza el scraping dependiendo de los parametros colocados
        public static Object getElementsByClass(string langAlias = "", int pageNum = 0)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            IEnumerable<HtmlAgilityPack.HtmlNode> nodes;
            
            try
            {
                // Si no se coloca ningun parametro, se usa para scraping de Tiobe Top 20
                if (String.IsNullOrEmpty(langAlias) && pageNum == 0)
                {
                    doc = web.Load("https://www.tiobe.com/tiobe-index/");
                    nodes = doc.DocumentNode.Descendants().Where(item => item.HasClass("td-top20"));
                }
                // Si solo se coloca el alias, se usa para scraping de cantidad de repositorios del lenguaje
                else if (!String.IsNullOrEmpty(langAlias) && pageNum == 0)
                {
                    doc = web.Load($"https://github.com/topics/{langAlias}");
                    nodes = doc.DocumentNode.Descendants().Where(item => item.HasClass("h3")).Where(item => item.HasClass("color-fg-muted"));
                }
                // Si se coloca un alias y el numero de pagina es mayor a 0 se usa para extraer la fecha y topics
                else if (!String.IsNullOrEmpty(langAlias) && pageNum > 0)
                {
                    doc = web.Load($"https://github.com/topics/{langAlias}?o=desc&s=updated&page={pageNum}");
                    nodes = doc.DocumentNode.Descendants().Where(item => item.HasClass("my-4"));
                }
                // Caso contrario, hubo un error al introducir los parametros
                else
                {
                    Console.WriteLine("Parametro introducido invalido");
                    return (int)-1;
                }
            }
            catch (System.Net.WebException err)
            {
                Console.WriteLine(err.Message);

                return (int)-1;
            }

            return nodes;
        }

        // Genera un archivo con el array de los tipos de datos soportados
        public static void generateFile(Object ObjArr, string fileName)
        {
            var filePath = $"data/{fileName}";

            // En caso que exista, borra el archivo
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            try
            {
                // Crea un archivo en la direccion especificada
                using (StreamWriter fileStr = File.CreateText(filePath))
                {
                    // Opcion para poder utilizar la funcion con otros tipos de datos
                    if (ObjArr is List<Language>)
                    {
                        foreach (var item in (List<Language>)ObjArr)
                        {
                            fileStr.WriteLine($"{item.getName()},{item.getRepoAmmount()}");
                        }
                    }
                    else if (ObjArr is IOrderedEnumerable<KeyValuePair<string, int>>)
                    {
                        foreach (var item in (IOrderedEnumerable<KeyValuePair<string, int>>)ObjArr)
                        {
                            fileStr.WriteLine($"{item.Key},{item.Value}");
                        }
                    }
                    else if (ObjArr is Dictionary<string, int>)
                    {
                        foreach (var item in (Dictionary<string, int>)ObjArr)
                        {
                            fileStr.WriteLine($"{item.Key},{item.Value}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Tipo de dato no soportado");
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("El archivo no puede abrirse, porfavor intente cerrar el archivo");
            }
        }

        // Imprime los elementos de un Dictionary y de un IOrderedEnumerable
        public static void printElements(Object elements)
        {
            if (elements is Dictionary<string, int>)
            {
                foreach (var element in (Dictionary<string, int>)elements)
                {
                    Console.WriteLine($"{element.Key}, {element.Value}");
                }
            }
            else if (elements is IOrderedEnumerable<KeyValuePair<string, int>>)
            {
                foreach (var element in (IOrderedEnumerable<KeyValuePair<string, int>>)elements)
                {
                    Console.WriteLine($"{element.Key}, {element.Value}");
                }
            }
            if (elements is List<Language>)
            {
                foreach (var element in (List<Language>)elements)
                {
                    Console.WriteLine($"{element.getName()}, {element.getRating()}, {element.getRepoAmmount()}");
                }
            }
            else
            {
                Console.WriteLine("Tipo de dato no soportado");
            }
        }

        // Ordena un diccionario retornando tipo Enumerable
        public static IOrderedEnumerable<KeyValuePair<string, int>> sortDictionary(Dictionary<string, int> dic)
        {
            return dic.OrderByDescending(x => x.Value);
        }
    }
}