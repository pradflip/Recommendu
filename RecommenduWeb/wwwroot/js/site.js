
function ListarCidades() {
    // pega o valor do campo selecionado
    var idTitulo = document.getElementById('srcTitulo').value;
    var idEstado = document.getElementById('ddlEstados').value;

    // refresh na tela
    location.reload()

    // passa para a url os parametros selecionados
    self.location = '?titulo=' + idTitulo + '&GetEstados=' + idEstado
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
            document.getElementById("divServicos").hidden = true;
        }
        else {
            document.getElementById("divProdutos").hidden = true;
        }
    }
    else {
        if (document.getElementById("divServicos").hidden === true) {
            document.getElementById("divServicos").hidden = false;
            document.getElementById("divProdutos").hidden = true;
        }
        else {
            document.getElementById("divServicos").hidden = true;
        }
    }
}

function ValidarArquivo() {
    const fileInput = document.getElementById('fileInput');
    const extensoes = /(\.jpg|\.jpeg|\.png)$/i;

    if (extensoes.test(fileInput.value)) {
        btnSubmit.click();
    }
    else {
        alert('Por favor, selecione um arquivo JPG, JPEG ou PNG.');
        fileInput.value = '';
    }
}

function EnviarImagem() {
    fileInput.click();
}