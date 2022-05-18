

namespace csharp_ej1
{
    class BarChart
    {
        // Crea un archivo png conteniendo un grafico segun los valores aportados
        public static void generateGraph(Object objArr, int size, string fileName)
        {
            int count = 0;
            var plt = new ScottPlot.Plot(1900, 1024);
            double[] positions = new double[size];
            string[] labels = new string[size];
            double[] values = new double[size];

            // Vector con la cantidad de valores
            for (int i = 0; i < size; i++)
            {
                positions[i] = (double)i;
            }

            // Tipos de datos soportados para crear el grafico
            if (objArr is List<Language>)
            {
                foreach (var item in (List<Language>)objArr)
                {
                    labels[count] = item.getName();
                    values[count] = item.getRepoAmmount();

                    if (++count >= size) break;
                }
            }
            else if (objArr is IOrderedEnumerable<KeyValuePair<string, int>>)
            {
                foreach (var item in (IOrderedEnumerable<KeyValuePair<string, int>>)objArr)
                {
                    labels[count] = item.Key;
                    values[count] = item.Value;

                    if (++count >= size) break;
                }
            }
            else if (objArr is Dictionary<string, int>)
            {
                foreach (var item in (Dictionary<string, int>)objArr)
                {
                    labels[count] = item.Key;
                    values[count] = item.Value;

                    if (++count >= size) break;
                }
            }
            else
            {
                Console.WriteLine("Tipo de dato no soportado");
            }

            // Configuracion del grafico y guardado
            plt.AddBar(values, positions);
            plt.XTicks(positions, labels);
            plt.SetAxisLimits(yMin: 0);
            plt.SaveFig(fileName);
        }
    }
}



