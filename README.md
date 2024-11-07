# Visão geral da aplicação

<br>

Esse projeto consiste na criação de uma plataforma para centralizar a busca de usuário por recomendação de produtos e serviços. O intuito é recomendar boas experiências, não sendo permitido postagens para reclamação.
No Recommendu, é possível criar postagens com título, descrição, foto, entre outras informações. O campo de descrição é avaliado pelo machine learning para detectar se seu comentário é positivo ou negativo referente à sua recomendação.
Além disso, outros usuários podem fazer comentários e curtir suas postagens.
Também é possível entrar no perfil do seu usuário e ver suas publicações, lista de postagens que curtiu ou acessar outros perfis.

<br>

https://github.com/user-attachments/assets/4b08f8f4-bc06-4a01-ad20-5f7e98a09074

<br>

# Manual de instalação

- Instalar .NET 7 SDK: https://dotnet.microsoft.com/pt-br/download/visual-studio-sdks
  - Apenas avançar as etapas.
  
- Instalar o Visual Studio 2022 Community: https://visualstudio.microsoft.com/pt-br/vs/community/
  - No instalador, ative essas 2 opções:
  
  ![image](https://github.com/pradflip/Recommendu/assets/99927329/d36b5fa8-8bf3-4bd0-b38b-cb6160594368)
  ![image](https://github.com/pradflip/Recommendu/assets/99927329/e1199c2f-8727-41b1-9931-64cb27752ee3)


- Instalar o SQLServer Express 2022: https://www.microsoft.com/pt-br/sql-server/sql-server-downloads
- Link direto: https://go.microsoft.com/fwlink/p/?linkid=2216019&clcid=0x416&culture=pt-br&country=br
  - Seguir com a instalação normalmente
  - Na janela de Central de instalação do SQL Server, selecione Instalação > Nova Instalação autonoma do SQL Server...
  - Clicar em avançar, na etapa Extensão do Azure desmarque o checkbox e prossiga.
  - Na etapa Seleção de recursos marque a opção LocalDB.
  - Em Configuração da Instância, deixe a Instancia nomeada "SQLExpress" e o Id "SQLEXPRESS".
  - Em Configuração do Mecanismo de Banco de Dados, selecione o modo de autenticação misto
  - Recomendação de senha: Tcc2023!
  - Nesta etapa, foi configurado 2 tipos de acesso: autenticação do windows e um login sql.
  - Com o login sql, foi configurado o usuário 'sa' e a senha que foi definida.
  - Concluir instalação.
  
  
- Instalar o Azure Data Studio: https://azure.microsoft.com/pt-br/products/data-studio
  - Apenas avançar as etapas.

- Realizar conexão com o banco de dados no Azure Data Studio:
  - Clicar em 'Create a connection'
  - Server: (localdb)\MSSQLLocalDB
  - Autenticação com Windows
    - Authentication type: Windows Authentication
  - Autenticação com login
    - Authentication type: SQL Login
    - User name: sa
    - Password: <senha definida na instalação do SQL Server>
  - Name = nomear a conexão (opcional)
  - Clicar em Connect

- Baixar projeto no repositório do Github
- Abrir o projeto executando a solution (.sln)

- Após abrir no Visual Studio, ir no menu superior em Compilação > Compilar Solução
- Em inglês: Build > Build Solution

- Criar estrutura do banco de dados
  - Ainda no Visual Studio, clique em Exibir > Outras janelas > Console do Gerenciador de Pacotes
  - Em inglês: View > Other Windows > Package Manager Console
  - Digitar no console "update-database" para gerar o banco de dados

- Caso apareca um popup sobre o certificado SSL do IIS Express, clique em sim
- Executar a aplicação (ctrl + shift + f5)
 - Se aparecer a janela 'Sua conexão não é particular', clique em avançado e ir para localhost.

- Para criar um perfil de administrador da aplicação, executar o comando sql no Azure Data Studio:
  select * from RecommenduWeb.dbo.AspNetUsers
- copie o id do seu usuário e execute o comando:
  insert into RecommenduWeb.dbo.AspNetUserRoles values ('(id do usuario)', 1);

- Para facilitar os testes, é possível desabilitar a confirmação de email, permitindo incluir emails inexistentes
  - No program.cs altere a "options.SignIn.RequireConfirmedAccount = true" para false
