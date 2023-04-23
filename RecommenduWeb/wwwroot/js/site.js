
function ListarCidades() {
    // pega o valor do campo selecionado
    var idTitulo = document.getElementById('srcTitulo').value;
    var idEstado = document.getElementById('ddlEstados').value;

    // refresh na tela
    location.reload()

    // passa para a url os parametros selecionados
    self.location = 'servicos?titulo=' + idTitulo + '&GetEstados=' + idEstado
}

function CarregaForm() {
    var categoria = document.getElementById('slcCategoria').value.toString();

    if (categoria === "1") {
        document.getElementById("formProduto").hidden = false;
        document.getElementById("formServico").hidden = true;
        document.getElementById("btnSubmit").disabled = false;
        document.getElementById("lblTitulo").innerHTML = "Nome do produto";
    }
    else if (categoria === "2") {
        document.getElementById("formProduto").hidden = true;
        document.getElementById("formServico").hidden = false;
        document.getElementById("btnSubmit").disabled = false;
        document.getElementById("lblTitulo").innerHTML = "Nome do serviço";
    }
    else {
        document.getElementById("formProduto").hidden = true;
        document.getElementById("formServico").hidden = true;
        document.getElementById("btnSubmit").disabled = true;
        document.getElementById("lblTitulo").innerHTML = "Título";
    }
}

function MostrarPost(cat) {
    if (cat === 1) {
        if (document.getElementById("divProdutos").hidden === true) {
            document.getElementById("divProdutos").hidden = false;
        }
        else {
            document.getElementById("divProdutos").hidden = true;
        }
    }
    else {
        if (document.getElementById("divServicos").hidden === true) {
            document.getElementById("divServicos").hidden = false;
        }
        else {
            document.getElementById("divServicos").hidden = true;
        }
    }
}