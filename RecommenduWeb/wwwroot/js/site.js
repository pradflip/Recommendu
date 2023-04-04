// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function teste() {
    // pega o valor do campo selecionado
    var idEstado = document.getElementById('ddlEstado').value
    if (idEstado.toString() === "0") {
        document.getElementById("ddlCidade").disabled = true;
    }
    else {
        document.getElementById("ddlCidade").disabled = false;
    }

    // pega o texto do valor selecionado
    var txtEstado = document.getElementById("ddlEstado").options[document.getElementById('ddlEstado').selectedIndex].text;

    alert("Id: " + idEstado + ", Sigla: " + txtEstado);
}
