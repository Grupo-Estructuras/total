from common.configuration import configure
from scraping.githubscraper import scrapeInterest
from graph.barChart import graphInterest


def main():
    # Leer configuración
    config = configure()

    # Obtener datos de github
    topics = scrapeInterest(config["scraper"], config["archivo_resultado"])
    for topic in topics:
        print(f"{topic[0]}:{topic[1]}")

    # Graphicar y mostrar
    graphInterest(topics)


main()
