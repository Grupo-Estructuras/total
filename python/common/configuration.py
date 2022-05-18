import argparse
import json
import logging


def configure():
    # Crear configuración por defecto en caso de que no existe un archivo
    defconfig = {
        "usar_lista_fija": False,
        "lista_lenguajes": [],
        "scraper": {
            "tiobe_site_format": "https://www.tiobe.com/tiobe-index/",
            "github_site_format": "https://github.com/topics/{}",
            "aliases": {
                "C#": "csharp",
                "C++": "cpp",
                "Classic Visual Basic": "visual-basic",
                "Delphi/Object Pascal": "delphi"
            },
            "retry_delays_ms": [
                300,
                600,
                1200
            ],
            "max_pages_interest": 10,
            "interest": "sort",
            "max_parallel": 5,
            "github_interest_format": "https://github.com/topics/{}?o=desc&s=updated&page={}"
        },
        "archivo_resultado": "data/Resultados.txt"
    }

    # Leer argumentos para ver que archivo de configuración usar
    parser = argparse.ArgumentParser(description="Parsear Github")
    parser.add_argument("-c", "--config", type=str, default="data/config.json")
    args = parser.parse_args()

    # Intentar abrir
    try:
        with open(args.config, "r+") as configfile:
            # Cargar valores que se encuentran en archivo
            try:
                defconfig.update(json.load(configfile))
            except json.decoder.JSONDecodeError as err:
                logging.warning(
                    f"No se reconoce el archivo de configuración: {err}. Sobreescribiendo...")
                pass

            # Volver a escribir (en caso de que alguna información no se encontraba inicialmente en el archivo)
            configfile.seek(0)
            json.dump(defconfig, configfile, indent=4)
            configfile.truncate()
    except IOError:
        logging.warning(
            "No se pudo abrir archivo. Usando configuración predeterminada...")

    return defconfig
