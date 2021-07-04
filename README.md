# Api DataDriven Asp.Net Core
Implementação de uma API orientada a dados usando:

 - .Net 5.0;
 - Asp.Net Core;
 - Swagger Documentation;
 - Entity Framework Core;
 - Entity Framework Core InMemory;
 - AutoMapper;
 - Authentication JwtBearer;
 
## Objetivo 

O intuito desse repositório é aplicar alguns dos conhecimentos absorvidos no curso "Criando APIs Data Driven" da plataforma Balta.io.

## Solução

O repositório contém uma soluação em .NET 5.0 onde foi implementado um pequeno projeto web api com Asp.NET Core. Além das operações tradicionais de crud normalmente econtradas em apis do gênero, a implementação permite algumas intereações entre três entidados principais(Product, Category e User). Além dessas interações e relacionamentos, a api realiza autenticações e autorizações para realizar algumas das operações expostas nos seus endpoints.

## Como usar ?

Tendo os requisistos mínimos para executar uma aplicação .NET 5.0 previamente instalados no local de execução, basta realizar o clone no repositório, acessar o seu diretório Shop via terminal e executar o comando "dotnet run".

Pronto ! a aplicação já está em funcionamento.

Você pode acessar o swagger documentation da api usando uma das portas informadas no log do terminal, exemplo:
https://localhost:5001/swagger

