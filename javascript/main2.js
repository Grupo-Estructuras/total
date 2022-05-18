const puppeteer = require('puppeteer');
const fs = require('fs');
const {ejecutarScrapGH2} = require('./scraping/githubscraper2.js');
const execfunc= require('child_process');

process.setMaxListeners(Infinity);

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
    await ejecutarScrapGH2(); //ejecuta todo el proceso
    execfunc.exec(getCommandLine() + ' index2.html'); //abre el navegador predeterminado para el grafico
  }

  iniciarProc();