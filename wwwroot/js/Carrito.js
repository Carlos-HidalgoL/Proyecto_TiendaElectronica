document.addEventListener('DOMContentLoaded', function () {
    mostrarCarrito();
});

function mostrarCarrito() {
    let carrito = JSON.parse(localStorage.getItem('carrito')) || [];

    const carritoBody = document.getElementById('carrito-body');
    carritoBody.innerHTML = '';

     let subtotal = 0;

    carrito.forEach(function (item) {
   
        let precio = parseInt(item.precio);

        let total = precio * item.cantidad;
        subtotal += total;
        let newRow = document.createElement('tr');

        newRow.innerHTML = `
            <td>
                <div class="d-flex align-items-center">
                    <img src="data:image;base64,${item.imagen}" class="img-fluid mr-3" style="max-width: 80px; height: auto; object-fit: cover;">
                    <div>
                        <h5 class="p-2 text-white">${item.nombre}</h5>
                    </div>
                </div>
            </td>
            <td class="align-middle text-white">${item.precio}</td>
            <td class="align-middle">
                <input type="number" class="form-control text-center" min="1" value="${item.cantidad}" onchange="actualizarCantidad(${item.id}, this.value)">
            </td>
            <td class="align-middle text-white">${total}</td>
            <td class="align-middle">
                <button class="btn btn-danger btn-sm" onclick="eliminarProducto(${item.id})">Eliminar</button>
            </td>
        `;

        carritoBody.appendChild(newRow);
    });

    document.getElementById('subtotal').textContent = `₡${subtotal}`;
    let iva = subtotal * 0.13;
    document.getElementById('iva').textContent = `₡${iva.toFixed(2)}`;
    document.getElementById('total').textContent = `₡${(subtotal + iva).toFixed(2)}`;
}

function eliminarProducto(id) {
    console.log('Eliminando producto con id:', id);
    let carrito = JSON.parse(localStorage.getItem('carrito')) || [];
    let nuevoCarrito = carrito.filter(item => item.id !== String(id));

    localStorage.setItem('carrito', JSON.stringify(nuevoCarrito));

    mostrarCarrito();
}


function actualizarCantidad(id, nuevaCantidad) {
    console.log('Actualizando cantidad del producto con id:', id, 'nueva cantidad:', nuevaCantidad);
    let carrito = JSON.parse(localStorage.getItem('carrito')) || [];
    carrito.forEach(item => {
        if (item.id === String(id)) {
            item.cantidad = parseInt(nuevaCantidad, 10);
        }
    });

    localStorage.setItem('carrito', JSON.stringify(carrito));

    mostrarCarrito();
}
