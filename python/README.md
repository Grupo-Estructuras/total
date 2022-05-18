# python (python3)
## Como configurar
Toda la configuración se guarda en un archivo de formato json. Se puede ver un ejemplo de una configuración por defecto en data/config.json


Esta configuración permite:
- Usar una lista fija para los lenguajes a buscar, definiendo ```usar_lista_fija: true``` y poniendo la lista como por ejemplo ```lista_lenguajes: [sle, python, c]```.
- Usar directamente la lista top20 de tiobe definiendo ```usar_lista_fija: false``` y definiendo las necesarias traducciones de tiobe a github en aliases (ver configuración por defecto para ejemplos).
- Definir el archivo donde se guarda el grafo
- Definir el archivo donde se guarda el resultado en texto

El archivo de configuración también se puede pasar como argumento de consola.

## Como ejecutar
Para ejecutar se necesita las librerias de requirements.txt. Se pueden instalar con ```pip install -r requirements.txt```

Además se puede instalar autopep8 que fue utilizado para el formato de todos los códigos.

Se recomienda Python 3.9.7 para ejecutar los códigos. 
Con ```python``` como Python 3.9.7 se ejecuta simplemente ```python ejercicio_1.py``` o ```python ejercicio_1.py``` desde el directorio de este archivo para probar ejercicio 1 o 2 del trabajo respectivamente.
