namespace csharp_ej1
{
    class Tiobescraper
    {
        public static List<string> scrapeTiobe()
        {
            // Se busca el elemento dentro de la pagina de Tiobe 
            List<string> languages = new List<string>();
            var generatedNode = Utilities.getElementsByClass();

            // En caso de un error devuelve un entero, caso positivo devuelve una coleccion de nodos
            if (generatedNode is int) return new List<string>();

            IEnumerable<HtmlAgilityPack.HtmlNode> nodes = (IEnumerable<HtmlAgilityPack.HtmlNode>)generatedNode;
            
            // De la lista de nodos, el nodo siguiente a cada nodo principal contiene el nombre del lenguaje
            foreach (var node in nodes)
            {
                languages.Add(node.NextSibling.InnerText);
            }

            // En caso de no encontrar la cantidad de lenguajes envia un mensaje
            if (languages.Count < 20)
            {
                Console.WriteLine("Aviso: No se encontraron 20 entradas en tiobe. Tratando seguir igual...");
            }

            // Retorna la lista con el nombre de los lenguajes Top 20
            return languages;
        }
    }
}