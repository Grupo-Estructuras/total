import logging
import requests
import common.exceptions
from bs4 import BeautifulSoup


def scrapeTiobe(link):
    tiobePage = requests.get(link)
    if tiobePage.status_code != 200:
        raise common.exceptions.RequestException()

    tiobesoup = BeautifulSoup(tiobePage.text, features="html.parser")

    top20_elem = tiobesoup.find_all(class_="td-top20")

    if len(top20_elem) < 20:
        logging.warning(
            "No se encontraron 20 entradas en tiobe. Tratando seguir igual...")

    return [language.find_next("td").text for language in top20_elem]
