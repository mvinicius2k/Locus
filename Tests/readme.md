Na ausencia de um mecanismo parecido com o WebApplicationFactory, fiz os testes de integração funcionando por meio do Testconteiners, que iniciam automaticamente um conjunto de conteiners (docker compose) e enviam requisições para atestar a resposta.

Os .yml usados não esperam que o depurador seja anexado, o objetivo é agilizar a execução dos testes, mas é possível configurar para esperar (veja a classe TestcontainerFixture).

