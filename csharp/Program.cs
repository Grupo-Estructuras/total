using System.Diagnostics;


namespace csharp_ej1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Genera los lenguajes Top 20 de TIOBE
            var languages = Tiobescraper.scrapeTiobe();
            
            // Parte 1
            // Scraping de Github para la cantidad de repositorios
            var orderedLanguages = Githubscraper.scrapeGithub(languages);
            Utilities.generateFile(orderedLanguages, "Resultados.txt");
            BarChart.generateGraph(orderedLanguages, 10, "bar_graph.png");
            Utilities.printElements(orderedLanguages);
            
            // Abre grafico parte 1
            new Process
            {
                StartInfo = new ProcessStartInfo("bar_graph.png")
                {
                    UseShellExecute = true
                }
            }.Start();

            // Parte 2
            // Scraping de Github para la cantidad de topics relacionados
            var dic = Githubscraper2.getTopics("sort");
            var sortedDic = Utilities.sortDictionary(dic);
            Utilities.generateFile(sortedDic, "Resultados2.txt");
            BarChart.generateGraph(sortedDic, 20, "bar_graph2.png");
            Utilities.printElements(sortedDic);

            // Abre grafico parte 2
            new Process
            {
                StartInfo = new ProcessStartInfo("bar_graph2.png")
                {
                    UseShellExecute = true
                }
            }.Start();
        }
    }
}
