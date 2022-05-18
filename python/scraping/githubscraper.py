from datetime import datetime
import logging
import re
import threading
import requests
import common.exceptions
import iso8601
from bs4 import BeautifulSoup


def scrapeGithub(languages, config, result_file_name):
    langList = []
    min = 0
    max = 0

    aliases = config["aliases"]

    # Verificar que archivo resultado se puede abrir
    resultfile = None
    try:
        resultfile = open(result_file_name, "w")
    except IOError:
        logging.error(
            "No se pudo abrir un archivo para resultados. No se guardarán los resultados!")

    # Preparar listas para threads y sus resultados
    gitpages = []
    threads = []
    for language in languages:
        try:
            langAlias = aliases[language]
        except KeyError:
            langAlias = language

        link = str.format(config["github_site_format"], langAlias)
        print("Accediendo a " + link)

        sem = threading.Semaphore(config["max_parallel"])
        threads.append(threading.Thread(
            target=language_read, args=(link, gitpages, sem, language)))

    # Accedemos a github en páginas paralelas
    for thread in threads:
        thread.start()
    for thread in threads:
        thread.join()

    # Procesar página por página
    for gitpage in gitpages:
        if gitpage[0].status_code != 200:
            print("Error página retorno código: " +
                  str(gitpage[0].status_code))
            raise common.exceptions.RequestException()

        githubsoup = BeautifulSoup(
            gitpage[0].text, features="html.parser")
        gitLenCant = githubsoup.find(class_="h3 color-fg-muted").text
        gitLenCant = re.search("\d+(,\d*)*", gitLenCant).group()
        gitLenCant = int(gitLenCant.replace(",", ""))

        min = min if min < gitLenCant and min != 0 else gitLenCant
        max = max if max > gitLenCant else gitLenCant

        langItem = {
            "name": gitpage[1],
            "repoAmmount": gitLenCant,
            "rating": 0
        }

        if resultfile != None:
            resultfile.write(
                langItem["name"] + "," + str(langItem["repoAmmount"]) + "\n")
        langList.append(langItem)

    # Si pudimos obtener acceso a archivo guardar
    if resultfile != None:
        resultfile.close()

    # Ordenar resultados
    return ratingSorter(min, max, langList)


def scrapeInterest(config, result_file_name):
    topics = {}
    topiclist = []
    gitpages = []
    threads = []

    # Verificar que archivo resultado se puede abrir
    resultfile = None
    try:
        resultfile = open(result_file_name, "w")
    except IOError:
        logging.error(
            "No se pudo abrir un archivo para resultados. No se guardarán los resultados!")

    for page in range(0, config["max_pages_interest"]):
        # Usamos página +1 ya que github empieza en página 1
        link = str.format(
            config["github_interest_format"], config["interest"], page+1)
        print("Accediendo a " + link)

        sem = threading.Semaphore(config["max_parallel"])
        threads.append(threading.Thread(
            target=interest_read, args=(link, gitpages, sem)))

    for thread in threads:
        thread.start()
    for thread in threads:
        thread.join()

    # Procesar página por página
    for gitpage in gitpages:
        if gitpage.status_code != 200:
            print("Error página retorno código: " + str(gitpage.status_code))
            raise common.exceptions.RequestException()

        githubsoup = BeautifulSoup(
            gitpage.text, features="html.parser")

        articles = githubsoup.find_all("article")
        for article in articles:
            timeelem = article.find("relative-time")
            if timeelem is None:
                continue

            time = iso8601.parse_date(
                timeelem["datetime"]).replace(tzinfo=None)
            if (datetime.utcnow() - time).days >= 30:
                continue

            tags = article.find_all("a", {"class": "topic-tag"})
            for tag in tags:
                key = re.sub("\s+", " ", tag.text)
                topics[key] = topics.get(key, 0) + 1
                if resultfile != None:
                    resultfile.write(key+"\n")

    # Guardar en lista
    for key in topics:
        topiclist.append((key, topics[key]))
    return sorted(topiclist, key=lambda i: i[1], reverse=True)


def language_read(link, pages, sem, language):
    sem.acquire()
    pages.append((requests.get(link), language))
    sem.release()


def interest_read(link, pages, sem):
    sem.acquire()
    pages.append(requests.get(link))
    sem.release()


# Adds rating to each item and returns the list sorted by rating
def ratingSorter(min, max, list):
    for item in list:
        item["rating"] = (item["repoAmmount"] - min) / (max - min) * 100

    return sorted(list, key=lambda i: i["rating"], reverse=True)
