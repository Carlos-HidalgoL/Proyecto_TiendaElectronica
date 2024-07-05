$(document).ready(() => {
    validarError();
});

const validarError = () => {
    const { innerText: error } = $("#error")[0];

    if (error) {

        Swal.fire({
            title: "Ingreso Fallido",
            text: error,
            icon: "error",
            confirmButtonColor: "#E14848"
        });
    }

}