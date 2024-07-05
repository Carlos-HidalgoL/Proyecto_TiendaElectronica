
function changeMainImage(imageSrc) {
    const mainImage = document.getElementById("mainImage");
    mainImage.src = 'data:image;base64,' + imageSrc;
}

function agregarAlCarrito(id, nombre, precio, imagen) {
    const cantidad = parseInt(document.getElementById("cantidad").value);

    var articulo = {
        id: id,
        nombre: nombre,
        precio: precio,
        cantidad: cantidad,
        imagen: imagen 
    };
    console.log('Imagen en base64:', imagen);

    const carrito = JSON.parse(localStorage.getItem('carrito')) || [];

    // Verificar si el artículo ya esta en el carrito
    let encontrado = false;
    for (let i = 0; i < carrito.length; i++) {
        if (carrito[i].id === articulo.id) {
            carrito[i].cantidad += articulo.cantidad; 
            encontrado = true;
            break;
        }
    }

    if (!encontrado) {
        carrito.push(articulo);
    }

    localStorage.setItem('carrito', JSON.stringify(carrito));


    console.log('Carrito actualizado:', carrito);

    location.reload();
}

function agregarProductoAlCarrito(id, nombre, precio, imagen) {
    const carrito = JSON.parse(localStorage.getItem('carrito')) || [];


    if (carrito[id]) {
        carrito[id].cantidad += 1;
    } else {
        carrito[id] = {
            id: id,
            nombre: nombre,
            precio: precio,
            imagen: imagen,
            cantidad: 1
        };
    }

    localStorage.setItem('carrito', JSON.stringify(carrito));
    alert('Producto añadido al carrito');
}