# C#

## Como Configurar
- La configuracion es casi fija, se pueden cambiar los alias de los lenguajes en el archivo ```langAliases.json``` dentro de la carpeta ```data``` en caso de que cambien los nombres de los lenguajes

## Instalaciones Previas
- Primeramente se debe instalar el framework de Dotnet para poder ejecutar el proyecto de C#, que se puede descargar desde la página ```https://dotnet.microsoft.com/en-us/download``` teniendo en cuenta el sistema operativo
- Dentro del directorio raíz instalamos las librerias con los siguientes comandos
  - ```dotnet add package ScottPlot --version 4.1.43```
  - ```dotnet add package Newtonsoft.Json --version 13.0.1```
  - ```dotnet add package HtmlAgilityPack --version 1.11.42```

## Como ejecutar
- Abrir el terminal en la carpeta raíz del proyecto, colocar el comando: ```dotnet run```

## Resultados
- Los resultados en formato .txt se encuentran en la direccion ```./data``` con los nombres ```Resultados.txt``` y ```Resultados2.txt```
- Los graficos se encuentran en la carpeta raiz y son archivos ```.png``` con los nombres ```bar_graph.png``` y ```bar_graph2.png```
  
  
