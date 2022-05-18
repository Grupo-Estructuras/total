import matplotlib.pyplot as plt


def graphLanguages(langDataArr):
    fig = plt.figure(figsize=(10, 6))
    langs = [language["name"] for language in langDataArr]
    repoAmmount = [language["repoAmmount"] for language in langDataArr]

    plt.bar(langs[0:10], repoAmmount[0:10], color='orange', width=0.420)
    plt.xlabel("Lenguajes")
    plt.ylabel("Apariciones")
    plt.show()


def graphInterest(tags):
    fig = plt.figure(figsize=(10, 6))
    tagnames = [tag[0] for tag in tags]
    nums = [tag[1] for tag in tags]

    plt.bar(tagnames[0:20], nums[0:20], color='orange', width=0.420)
    plt.xlabel("Topics asociados")
    plt.ylabel("Apariciones")
    plt.show()
