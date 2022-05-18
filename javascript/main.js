const puppeteer = require('puppeteer');
const fs = require('fs');
const {ejecutarScrapGH} = require('./scraping/githubscraper.js');
const execfunc= require('child_process');
process.setMaxListeners(Infinity); //para llamar a las URL las veces que desee



function getCommandLine() { //determina el navegador predeterminado segun el SO para abrirlo
  switch (process.platform) {
    case 'darwin':
      return 'open';
    case 'win32':
      return 'start';
    case 'win64':
      return 'start';
    default:
      return 'xdg-open';
  }
}

async function iniciarProc(){
  await ejecutarScrapGH(); //ejecuta todo el proceso
  execfunc.exec(getCommandLine() + ' index.html'); //abre el navegador predeterminado para el grafico
}
iniciarProc();
