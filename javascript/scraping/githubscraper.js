const puppeteer = require('puppeteer');
const fs = require('fs');

function sleep(ms) { //para esperar y que no de error por demasiadas consultas
  return new Promise(resolve => setTimeout(resolve, ms));
}


async function scrapGitHub(urlGH, language) {
  try {
    const navegador = await puppeteer.launch();  // abrimos navegador
    const pagina = await navegador.newPage();    // abrimos pagina
    respuesta = await pagina.goto(urlGH);  // vamos al url y esperamos a que responda
    //abrimos el HTML de la pagina para buscar alli
    if (respuesta.status() != 200 ){ //200 es codigo de exito
      console.log ("No se pudo acceder a la pagina, se salta el lenguaje "+language+ " error "+respuesta.status());
    }else{
      console.log ("Scraping de "+language); // se abrio la pagina
      await sleep(5000);
    }
    
    descripcion = await pagina.evaluate(() => { 
      return document.querySelector('.h3.color-fg-muted').textContent; //buscamos el que cumpla con esta etiqueta, pues tiene la descripcion
    })

    numero = (/(\d+(,\d*)*)/).exec(descripcion); //tomamos el numero de apariciones con regex, se guarda en la posicion 0
    await navegador.close(); //cerramos el navegador
    numero[0] = numero[0].replace(',', ''); //borramos la coma

    return numero[0]; //retorna la cantidad de apariciones
  } catch (error) {
    console.error();
    return 0;
  }
}
function calcRating(languages, min, max) {
  // calculamos rating
  for (var i = 0; i < 20; i++) {
    languages[i].rating = ((languages[i].apar - min) / (max - min)) * 100;
  }
  // ordenamos
  languages.sort((a, b) => {
    if (a.rating == b.rating) {
      return 0;
    }
    if (a.rating > b.rating) {
      return -1;
    }
    return 1;
  });
  // imprimimos como se pide
  for (let i = 0; i < 20; i++) {
    console.log(
        languages[i].nombre + ', ' + languages[i].rating + ', ' +
        languages[i].apar);
  }
  //guardamos los datos para le grafico en abcisa y ordenada
  let abcisa = [];
  let ordenada = [];
  for (var i = 0; i < 10; i++) {
    abcisa.push(languages[i].nombre); //nombres
    ordenada.push(languages[i].apar); //apariciones
  }
  let archivo = `
    var abcisa=${JSON.stringify(abcisa)};
    var ordenada=${JSON.stringify(ordenada)};`; //en una cadena guardamos el contenido de la abcisa y ordenada
  fs.writeFileSync('./scraping/datos.js', archivo, {flag: 'w'}); //creamos un archivo .js para guardar esos datos
}


async function desaliasing(languages) {
  min = -1;
  max = 0;
  totalEntry = '';
  try {
    AliasJson = fs.readFileSync('./data/langAliases.json', 'utf-8'); //abrimos el archivo json con los alias
    AliasJson = JSON.parse(AliasJson);//pasamos a objeto

    for (var i = 0; i < 20; i++) { //recorreremos cada lenguaje
      linkear = 'https://github.com/topics/' + AliasJson[languages[i].nombre]; //link en github por lenguaje ya usando el alias

      languages[i].apar = Number(await scrapGitHub(linkear, languages[i].nombre)); // llamamos a la funcion que busca el numero de apariciones y retorna dicho numero

      totalEntry =
          totalEntry + languages[i].nombre + ', ' + languages[i].apar + '\n'; //guarda el string de lo que se escribira en el archivo resultado
      
      if (min > languages[i].apar || min<0) { //calculo minimo y maximo
        min = languages[i].apar;
      }
      if (max < languages[i].apar) {
        max = languages[i].apar;
      }

    }
    fs.writeFileSync(
        './data/Resultados.txt', totalEntry,
        {flag: 'w'});  // escribimos los resultados
    await calcRating(languages, min, max); //calculamos los rating
  } catch (err) {
    console.error(err);
  }
}


async function ejecutarScrapGH() {  // aca empezamos
  let languages = [
    {nombre: 'python', apar: 0, rating: 0}, 
    {nombre: 'c', apar: 0, rating: 0},
    {nombre: 'java', apar: 0, rating: 0},
    {nombre: 'c++', apar: 0, rating: 0},
    {nombre: 'c#', apar: 0, rating: 0},
    {nombre: 'visual basic', apar: 0, rating: 0},
    {nombre: 'javascript', apar: 0, rating: 0},
    {nombre: 'assembly language', apar: 0, rating: 0},
    {nombre: 'sql', apar: 0, rating: 0},
    {nombre: 'php', apar: 0, rating: 0},
    {nombre: 'r', apar: 0, rating: 0},
    {nombre: 'delphi/object pascal', apar: 0, rating: 0},
    {nombre: 'go', apar: 0, rating: 0},
    {nombre: 'swift', apar: 0, rating: 0},
    {nombre: 'ruby', apar: 0, rating: 0},
    {nombre: 'classic visual basic', apar: 0, rating: 0},
    {nombre: 'objective-c', apar: 0, rating: 0},
    {nombre: 'perl', apar: 0, rating: 0},
    {nombre: 'lua', apar: 0, rating: 0},
    {nombre: 'matlab', apar: 0, rating: 0},
  ]; //lista de lenguajes
  await desaliasing(languages); //vamos a buscar los alias por cada lenguaje
}

module.exports = { //exportamos para usar en otro archivo

  ejecutarScrapGH,
};