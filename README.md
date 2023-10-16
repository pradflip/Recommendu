# Recommendu
# Manual de instalação

- Instalar .NET 7 SDK: https://dotnet.microsoft.com/pt-br/download/visual-studio-sdks
- Instalar o Visual Studio 2022 Community: https://visualstudio.microsoft.com/pt-br/vs/community/
- Instalar o SQLServer Express: https://www.microsoft.com/pt-br/sql-server/sql-server-downloads
- Instalar o Azure Data Studio: https://azure.microsoft.com/pt-br/products/data-studio

- Após baixar o projeto e abrir no Visual Studio, ir no menu superior esquerdo em View > Other Windows > Package Manager Console
- Digitar no console "update-database" para gerar o banco de dados
- Executar a aplicação

- Para criar um perfil de administrador da aplicação, executar o comando sql no Azure Data Studio:
  select * from RecommenduWeb.dbo.AspNetUser (copie o id do seu usuário)
  insert into RecommenduWeb.dbo.AspNetUserRoles ({id do usuario}, 1)
