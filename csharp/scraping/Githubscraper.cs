using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace csharp_ej1
{
    class Githubscraper
    {
        // Busca la cantidad de repositorios que posee cada lenguage de la lista
        public static List<Language> scrapeGithub(List<string> languages)
        {       
            List<Language> langObjArr = new List<Language>();
            long min = long.MaxValue;
            long max = long.MinValue;
            
            try 
            {
                // Se abre un archivo con los alias y se convierte en un objeto
                using (StreamReader file = File.OpenText("data/langAliases.json"))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject aliasData = (JObject)JToken.ReadFrom(reader);
                    JToken alias;
                    
                    foreach (var language in languages)
                    {   
                        // Se prueba si existe un alias, caso negativo se usa el original
                        if (!aliasData.TryGetValue(language.ToLower(), out var value))
                        {
                            value = (JToken)language;
                        }
                        
                        alias = value.ToString();
                        
                        // Busca la cantidad de repositorios del lenguaje con el alias
                        long repoAmmount = (long)0;
                        var maxTry = 3;
                        
                        try
                        {
                            repoAmmount = getRepoAmmount(alias.ToString());
                        }
                        catch (System.Exception)
                        {
                            if (maxTry-- > 0)
                            {
                                Console.WriteLine("Sleeping");
                                Thread.Sleep(3000);
                                Console.WriteLine("Woke up");
                                repoAmmount = getRepoAmmount(alias.ToString());
                            }
                            else
                            {
                                Console.WriteLine("Cantidad maxima de intentos, cerrando...");
                                throw;
                            }
                        }
                        

                        // Se buscan los valores minimos y maximos de repositorios
                        min = (min < repoAmmount) ? min : repoAmmount;
                        max = (max > repoAmmount) ? max : repoAmmount;

                        // Se crea una estructura de lenguaje y se agrega el lenguaje a un array
                        Language langObj = new Language(language, repoAmmount, 0.0);
                        langObjArr.Add(langObj);

                        Console.WriteLine($"Scraping...{language}");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("No posee perimsos para abrir el archivo");
                Environment.Exit(0);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("El archivo no se encuentra, verifique la ruta del archivo");
                Environment.Exit(0);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("No se encuentra el directorio del archivo, verifique la ruta del directorio");
                Environment.Exit(0);
            }

            return updateRatingSorted(langObjArr, min, max);
        }

        // Agrega el rating a cada lenguaje y retorna la lista de lenguajes ordenada por cantidad de repositorios
        private static List<Language> updateRatingSorted(List<Language> langObjArr, long min, long max)
        {
            List<Language> tempLangArr = new List<Language>();

            // Aplica la formula para el rating y agrega el atributo a cada lenguaje
            foreach (var item in langObjArr)
            {
                var newRating = Math.Round((double)(item.getRepoAmmount() - min) / (max - min) * 100, 3);
                tempLangArr.Add(new Language(item.getName(), item.getRepoAmmount(), newRating));
            }

            // Ordena la lista de lenguajes segun cantidad de repositorios
            tempLangArr.Sort((Language item1, Language item2) => item2.getRepoAmmount().CompareTo(item1.getRepoAmmount()));

            return tempLangArr;
        }

        // Retorna la cantidad de repositorios que posee un lenguaje
        private static long getRepoAmmount(string langAlias)
        {
            // Se busca el elemento dentro de la pagina segun la clase especificada
            var generatedNode = Utilities.getElementsByClass(langAlias);

            // En caso de un error devuelve un entero, caso positivo devuelve una coleccion de nodos
            if (generatedNode is int) return 0;

            IEnumerable<HtmlAgilityPack.HtmlNode> nodes = (IEnumerable<HtmlAgilityPack.HtmlNode>)generatedNode;

            // Se selecciona el ultimo item y se guarda el html para buscar con regex la cantidad
            var foundData = nodes.Last().InnerText;
            var repoAmmount = Regex.Match(foundData, @"\d+(,\d*)*").Value;
            repoAmmount = repoAmmount.Replace(",", "");

            // Retorna la cantidad convertida en entero
            return Int64.Parse(repoAmmount);
        }
    }
}