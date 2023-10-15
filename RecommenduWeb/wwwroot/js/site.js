
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

function ValidarArquivo() {
    const fileInput = document.getElementById('fileInput');
    const extensoes = /(\.jpg|\.jpeg|\.png)$/i;

    if (extensoes.test(fileInput.value)) {
        btnPerfilSubmit.click();
    }
    else {
        alert('Por favor, selecione um arquivo JPG, JPEG ou PNG.');
        fileInput.value = '';
    }
}

function EnviarImagem() {
    fileInput.click();
}

const fileInput = document.getElementById('fileInput');
const uploadedImage = document.getElementById('imgNovoPost');

fileInput.addEventListener('change', function () {
    const file = fileInput.files[0];

    if (file) {
        const reader = new FileReader();

        reader.onload = function (e) {
            uploadedImage.src = e.target.result;
            uploadedImage.style.display = 'block';
        };
        reader.readAsDataURL(file);
        document.getElementById('btnNovoPost').style.display = 'none';
        document.getElementById('divClose').style.display = 'inline';

    } else {
        uploadedImage.src = '#';
        uploadedImage.style.display = 'none';
    }
});

function RemoverImagemForm() {
    fileInput.value = '';
    uploadedImage.src = '';
    uploadedImage.style.display = 'none';
    document.getElementById('btnNovoPost').style.display = 'inline';
    document.getElementById('divClose').style.display = 'none';
}