// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function teste() {
    // pega o valor do campo selecionado
    var estado = document.getElementById('ddlEstado').value
    if (estado.toString() === "0") {
        document.getElementById("ddlCidade").disabled = true;
    }
    else {
        document.getElementById("ddlCidade").disabled = false;
    }

    // pega o texto do valor selecionado
    estado = document.getElementById("ddlEstado").options[document.getElementById('ddlEstado').selectedIndex].text;

    alert(estado);
}
