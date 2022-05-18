const puppeteer = require('puppeteer');
const fs = require('fs');
var topics=[];
topic_elegido="sort";

function sleep(ms) { //para esperar y que no de error por demasiadas consultas
    return new Promise(resolve => setTimeout(resolve, ms));
 }

async function scrapGitHub(urlGH) {
    try {
        seguir=0;
        const navegador = await puppeteer.launch();  // abrimos navegador
        const pagina = await navegador.newPage();    // abrimos pagina
        respuesta = await pagina.goto(urlGH);  // vamos al url y esperamos a que responda
        if (respuesta.status() != 200 ){ //200 es codigo de exito
            console.log ("No se pudo acceder a la pagina, error "+respuesta.status());
        }else{
            console.log ("Scraping de la siguiente pagina"); // se abrio la pagina
            await sleep(5000);
        }
        
        fechasXArt=await pagina.evaluate(() => { //buscamos las fechas por articulos
            retorno=[];
            fechas=document.querySelectorAll('article div.p-3 ul li:first-child');

            fechas.forEach(function(FechaActual) {
                retorno.push(FechaActual.innerHTML); //sacamos el HTML alli esta la fecha
            });

            return retorno; //retornamos el array que tiene las fechas
        })

        topicsXArt=await pagina.evaluate(() => { //buscamos los topics agrupados por articulos
            retorno=[];
            topicsArt=document.querySelectorAll('article div.d-flex.flex-wrap.border-bottom.color-border-muted.px-3.pt-2.pb-2');

            topicsArt.forEach(function(topicsA) {
                retorno.push(topicsA.textContent); //queremos el text content alli estan los topics
            });

            return retorno;
        })

        if (fechasXArt.length==0){ //para ver si ya es una pagina sin nada
            seguir=1;
        }

        var myRe = /\w+/g; //regex, solo toma los alfanumericos
        fechaHoy=Date.now(); //fecha de hoy
        for (var i=0;i<fechasXArt.length;i++){
            FechaActual=fechasXArt[i]; //tomamos la fecha del articulo i

            fechaRegex=(/(\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\dZ)/).exec(FechaActual); //extraemos la fecha con regex
            fechaArt=(new Date(fechaRegex[0])).getTime(); //pasamos la fecha a milisegundos
            fechaHoy=fechaHoy-2592000000;
            if ( fechaHoy <= fechaArt){ //vemos si esta en el rango de 30 dias
                while ((unTopic = myRe.exec(topicsXArt[i])) !== null) { //veremos los topics utilizando regex para extraer
                    topicUnico =unTopic[0]; //topic se guarda en la posicion 0
                    indexTopic=topics.map(object => object.nombre).indexOf(topicUnico); //vemos si esta en la lista de vistos o no
                    
                    if (indexTopic>=0){ //ya esta en la lista
                        topics[indexTopic].cantidad=topics[indexTopic].cantidad + 1; //aumenta la cantidad
                    }else{ //es nuevo
                        topics.push({nombre:topicUnico,cantidad:1}); //agregamos
                    }
                    
                }
            }else{
                seguir=1; //ya no hay porque seguir
                break; //salimos
            }
        }

        await navegador.close();
        return seguir; //retorna si aun hace falta seguir scrapeando o no
    } catch (error) {
      console.error();
      return 1;
    }
}

async function ejecutarScrapGH2() {  // aca empezamos
    var ban=0;
    var pagina=1;
    //recorremos cada pagina, 30 como maximo p hasta que no hayan mas resultados posibles
    while (ban==0 && pagina<=30){
        linkear = "https://github.com/topics/"+topic_elegido+"?o=desc&s=updated&page=" + pagina;
        ban=await scrapGitHub(linkear); //retorna si aun hay que escrapear
        pagina=pagina+1;
    }
    //ordenamos
    topics.sort((a, b) => {
        if (a.cantidad == b.cantidad) {
          return 0;
        }
        if (a.cantidad > b.cantidad) {
          return -1;
        }
        return 1;
    });

    let totalEntry="";
    let abcisa=[];
    let ordenada=[];
    for (var i=0 ; i<topics.length;i++){ //veremos los topics
        totalEntry=totalEntry+topics[i].nombre+"\n"; //guardamos en un string para escribir despues
        if (i<20){
            abcisa.push(topics[i].nombre); //ponemos en las listas para el grafico
            ordenada.push(topics[i].cantidad);
        }
        console.log(topics[i].nombre+", "+topics[i].cantidad); //imprimimos lo solicitado
        
    }
    let archivo = `
        var abcisa=${JSON.stringify(abcisa)};
        var ordenada=${JSON.stringify(ordenada)};`;//en una cadena guardamos el contenido de la abcisa y ordenada

    fs.writeFileSync('./data/Resultados2.txt', totalEntry,{flag: 'w'});  //guardamos la lista de topics
    fs.writeFileSync('./scraping/datos2.js', archivo, {flag: 'w'}); //guardamos los datos de topics con apariciones
    
}
  
module.exports = {
  ejecutarScrapGH2,
};