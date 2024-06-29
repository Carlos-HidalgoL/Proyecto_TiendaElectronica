import { ObtenerDatos } from './ObtenerDatos.js';

$(document).ready(() => {

    cargarArticulos();

});


const cargarArticulos = async () => {

    const ruta = "/Admin/ObtenerArticulos";
    console.log(await ObtenerDatos(ruta));

    
};


