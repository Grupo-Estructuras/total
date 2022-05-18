using System.Text.RegularExpressions;


namespace csharp_ej1
{
    class Githubscraper2
    {
        // Retorna un diccionario con los topics que cumplen el criterio establecido
        public static Dictionary<string, int> getTopics(string langAlias)
        {
            Dictionary<string, int> topicsDic = new Dictionary<string, int>();
            // Configuracion basica (no tengo garra para hacer un archivo)
            int maxPage = 30;
            int maxDays = 30;
            bool ignoreMainTopic = false;

            // Se recorren las paginas y se actua dependiendo del codigo enviado
            for (int currPage = 1; currPage <= maxPage; currPage++)
            {
                var statusCode = getTopicByPage(topicsDic, langAlias, currPage, maxDays, ignoreMainTopic);
                
                if (statusCode == 1)
                {
                    Console.WriteLine("No hay mas elementos entre la franja de dias");
                    break;
                }
                else if (statusCode == -1)
                {
                    Console.WriteLine("Error, se aborto el programa");
                    return new Dictionary<string, int>();
                }
                else if (statusCode == 0 && currPage == maxPage)
                {
                    Console.WriteLine("Se llego al limite de paginas");
                }
            }

            Console.WriteLine("Se termino de manera satisfactoria");

            return topicsDic;
        }

        // Busca los topics por pagina, agregando los valores al diccionario cuando sean menores a la fecha
        private static int getTopicByPage(Dictionary<string, int> topicsDic, string langAlias, int pageNum, int maxDays, bool ignoreMainTopic)
        {
            // Se busca el elemento dentro de la pagina segun la clase especificada segun el numero de pagina introducido
            var generatedNode = Utilities.getElementsByClass(langAlias, pageNum);

            // En caso de un error devuelve un entero, caso positivo devuelve una coleccion de nodos
            if (generatedNode is int) return -1;

            IEnumerable<HtmlAgilityPack.HtmlNode> nodes = (IEnumerable<HtmlAgilityPack.HtmlNode>)generatedNode;

            // Recorre cada nodo que contiene el div con la fecha y los topics
            foreach (var node in nodes)
            {
                // Se guarda el html del div y se busca la fecha
                var nodeData = node.InnerHtml;
                var timeData = Regex.Match(nodeData, "mr-4[\\S\\s]*datetime=\"[^\"]+").Value;
                timeData = Regex.Match(timeData, "datetime=\"[^\"]+").Value;
                timeData = Regex.Match(timeData, "[^\"]*$").Value;

                // Si encuentra el patron de fecha, se buscan los topics relacionados
                if (!String.IsNullOrEmpty(timeData))
                {
                    DateTime enteredDate = DateTime.Parse(timeData);
                    TimeSpan difDate = DateTime.Now - enteredDate;

                    // Si la diferencia de dias es mayor o igual al maximo de dias
                    if (difDate.Days >= maxDays)
                        return 1;
                    
                    // Se buscan los topics asociados al div
                    var topics = Regex.Matches(nodeData, "Topic: [^\"]+");

                    foreach (var topic in topics)
                    {
                        var strTopic = topic.ToString();

                        // Revisa si encuentra un topic
                        if (!String.IsNullOrEmpty(strTopic))
                        {
                            var cleanTopic = Regex.Match(strTopic, "[^ ]*$").Value;

                            // Se ignora el topic con el nombre principal (opcional)
                            if (ignoreMainTopic && cleanTopic == langAlias) continue;

                            // Se agrega el topic al diccionario sumando las entradas e ignorando el nombre topic
                            if (!topicsDic.TryGetValue(cleanTopic, out var value))
                            {
                                topicsDic[cleanTopic] = 1;
                            }
                            else
                            {
                                topicsDic[cleanTopic]++;
                            }
                        }
                    }
                }
            }
            
            return 0;
        }
    }
}