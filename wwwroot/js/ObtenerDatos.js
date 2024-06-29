export const ObtenerDatos = async (ruta) => {

    let datos = [];
    try {
        await $.ajax({
            url: ruta,
            type: "GET",
            dataType: "json",
        }).done(function (data) {
            datos = datos;
        });
    } catch (error) {
        console.log(error);
    }

    return [];
} 