import logging
from common.configuration import configure
from scraping.tiobescraper import scrapeTiobe
from scraping.githubscraper import scrapeGithub, scrapeInterest
from graph.barChart import graphLanguages
from common import exceptions


def main():
    # Se lee archivo configuración
    config = configure()

    languages = []
    if not config["usar_lista_fija"]:
        try:
            languages = scrapeTiobe(config["scraper"]["tiobe_site_format"])
        except exceptions.RequestException as err:
            logging.error(
                f"No se pudo conectar con la página tiobe. Verifique su conexión. Error: {err}")
            return -1
    else:
        languages = config["lista_lenguajes"]

    # Obtener información de github
    try:
        langDataArr = scrapeGithub(
            languages, config["scraper"], config["archivo_resultado"])
    except exceptions.RequestException as err:
        logging.error(
            f"No se pudo conectar con la página github. Verifique su conexión. Error: {err}")
        return -1

    position = 0
    for language in langDataArr:
        position += 1
        print(
            f"{str(position)} - {language['name']},{language['rating']},{language['repoAmmount']}")

    # Graficar y mostrar en pantalla
    graphLanguages(langDataArr)


main()
