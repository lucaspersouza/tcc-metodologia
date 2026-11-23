Experimento do TCC: classificação de mensagens com e sem IA
Este projeto compara o custo computacional e energético de dois classificadores de mensagens de atendimento corporativo, escritos em C# (.NET 10):
`rules`: sistema sem IA, que classifica por regras de palavras-chave (22 termos) usando expressões regulares (Regex) criadas e compiladas uma única vez, na inicialização do classificador.
`ai`: sistema com IA local, que usa Naive Bayes multinomial treinado com um corpus sintético separado (suavização de Laplace, soma de log-probabilidades e remoção de palavras de uso muito comum).
Há ainda uma variante de controle, `rules-direct`, com a mesma lógica e as mesmas respostas de `rules`, mas na implementação original, em que o padrão de Regex é montado novamente a cada verificação. Ela existe no código, porém não faz parte das medições do artigo.
O programa executa um método por vez, para que o registro do HWiNFO possa ser iniciado e parado separadamente para cada execução. Não são usados dados reais, serviços externos ou APIs. As mensagens e os rótulos são fictícios.
Arquivos do projeto
`program.cs`: lê os parâmetros, executa o método escolhido, imprime os marcadores de horário e grava os CSVs.
`message-generator.cs`: contém as frases dos três conjuntos (treinamento, desenvolvimento e teste) e gera a base de avaliação.
`deterministic-classifier.cs`: classificador `rules` (e a interface `IMessageClassifier`).
`direct-regex-classifier.cs`: variante de controle `rules-direct`.
`naive-bayes-classifier.cs`: classificador `ai`.
`batch-processor.cs`: processa o lote várias vezes e mede tempo, CPU do processo e memória.
`csv-report.cs`: grava os arquivos CSV.
`message.cs`: define a mensagem e as categorias (Suporte, Vendas, Financeiro e Outros).
Dados sintéticos
Há três conjuntos de frases, mantidos separados. A cada execução, o programa confere que nenhuma frase se repete entre eles e interrompe a execução caso isso aconteça.
Treinamento (somente `ai`): 100 frases, 25 por categoria.
Desenvolvimento: 20 frases, usadas como referência para escrever as palavras-chave das regras.
Teste: 120 frases, 30 por categoria, usadas somente para avaliar os classificadores.
A base de avaliação tem 6.000 mensagens: cada frase de teste aparece 50 vezes, em ordem embaralhada de forma reproduzível pela semente (padrão 42). Se `--count` não for múltiplo de 120, algumas frases aparecem uma vez a mais.
Preparação (uma vez)
Compile o projeto antes das medições, para que o custo da compilação não entre no registro do HWiNFO. Só é preciso compilar de novo se o código for alterado.
```powershell
dotnet build -c Release
```
Como executar uma medição
Use sempre a mesma quantidade de mensagens, semente e repetições nos dois métodos. O protocolo do artigo usa 6.000 mensagens e 3.000 repetições, o que dá 18.000.000 de classificações por execução.
Deixe a CPU voltar para perto de 50 °C e feche os programas que não forem necessários.
Inicie o registro dos sensores no HWiNFO (modo somente sensores, intervalo de 500 ms, com os sensores Potência total da CPU, Relógio efetivo médio, Uso total da CPU e CPU Tctl/Tdie).
Aguarde cerca de 60 segundos com o computador em repouso, para registrar o consumo basal.
Execute o método (exemplo da rodada 1, no PowerShell):
```powershell
   dotnet run -c Release --no-build -- --method rules --count 6000 --seed 42 --repetitions 3000 --output resultados/rules-r1-mensagens.csv --metrics resultados/rules-r1-metricas.csv | Tee-Object -FilePath rules-r1-console.txt
   ```
```powershell
   dotnet run -c Release --no-build -- --method ai --count 6000 --seed 42 --repetitions 3000 --output resultados/ai-r1-mensagens.csv --metrics resultados/ai-r1-metricas.csv | Tee-Object -FilePath ai-r1-console.txt
   ```
Quando o programa terminar, aguarde cerca de 10 segundos e pare o registro do HWiNFO.
Salve o CSV do HWiNFO com um nome que identifique o método e a rodada, por exemplo `rules-r1.csv` e `ai-r1.csv`.
Os parâmetros `--output` e `--metrics` dão nomes próprios aos arquivos de cada rodada. Sem eles, cada execução sobrescreve os arquivos `resultados/<método>-mensagens.csv` e `resultados/<método>-metricas.csv`. O `Tee-Object` mostra o texto na tela e grava uma cópia em arquivo, o que preserva os horários dos marcadores.
Rodadas
O protocolo prevê 6 rodadas por método (12 execuções). A ordem alterna a cada rodada para reduzir o efeito do aquecimento: nas rodadas ímpares (1, 3 e 5) executa-se `rules` primeiro e `ai` depois, e nas pares (2, 4 e 6) executa-se `ai` primeiro e `rules` depois. Entre uma execução e a seguinte, espere a temperatura da CPU voltar para perto de 50 °C.
Marcadores de horário
O console imprime marcadores com o horário local: `EXPERIMENTO_INICIO`, `TREINAMENTO_INICIO` e `TREINAMENTO_FIM` (somente no `ai`), `CLASSIFICACAO_INICIO`, `CLASSIFICACAO_FIM` e `EXPERIMENTO_FIM`.
Use `CLASSIFICACAO_INICIO` e `CLASSIFICACAO_FIM` para delimitar, no registro do HWiNFO, o intervalo da classificação. A geração das mensagens, o treinamento do modelo e a gravação dos arquivos ficam fora desse intervalo, assim como a criação dos padrões de Regex do `rules`. Se quiser o custo total, considere o intervalo entre `EXPERIMENTO_INICIO` e `EXPERIMENTO_FIM`.
Arquivos gerados
Em cada execução, o programa grava:
`<método>-mensagens.csv`: uma linha por mensagem, com o texto, a categoria esperada, a categoria classificada e a indicação de acerto. Como a base é a mesma em todas as repetições e as respostas são determinísticas, o padrão de acertos e erros é idêntico em todas as rodadas.
`<método>-metricas.csv`: uma linha com o método, as repetições, o total de classificações, o tempo de treinamento, o tempo de classificação, o tempo de CPU do processo, o Working Set e o heap gerenciado antes e depois, os acertos, os erros, a acurácia e a vazão (mensagens por segundo).
O programa não controla nem exporta dados do HWiNFO: os registros de sensores são arquivos separados, gerados pelo próprio HWiNFO.
Para reunir as 12 linhas de métricas em um único arquivo e os 12 consoles em um único texto:
```powershell
$f = Get-ChildItem resultados\*-r?-metricas.csv
Get-Content $f[0] -TotalCount 1 | Set-Content resultados\todas-metricas.csv
$f | ForEach-Object { Get-Content $_ | Select-Object -Skip 1 } | Add-Content resultados\todas-metricas.csv
Get-Content *-console.txt | Set-Content todos-consoles.txt
```
Cálculo da energia
A energia (J) é a integral da potência no intervalo de classificação, estimada pelo método dos trapézios sobre as amostras do HWiNFO, com interpolação linear nos instantes dos marcadores: E ≈ Σ (Pᵢ + Pᵢ₊₁)/2 · Δtᵢ. A energia por classificação é E dividido pelo total de classificações da execução (mensagens × repetições). A energia bruta é a métrica principal; a potência média do período de repouso serve como referência. Frequência, utilização e temperatura são registradas como contexto e não entram no cálculo da energia.
Parâmetros
```text
dotnet run -- --method rules|rules-direct|ai [--count 6000] [--seed 42] [--repetitions 20] [--output caminho.csv] [--metrics caminho.csv]
```
Os valores padrão são 6.000 mensagens, semente 42 e 20 repetições. Para as medições do artigo, use `--repetitions 3000`, para que a classificação dure o suficiente para gerar muitas amostras de sensores.
Limites acadêmicos
A base e o corpus de treinamento são sintéticos, e o conjunto de teste tem apenas 120 frases distintas. A acurácia mede somente esses exemplos e não demonstra desempenho em mensagens reais. As palavras-chave das regras foram definidas a partir do conjunto de desenvolvimento, e as frases de teste foram elaboradas para não favorecer artificialmente nenhum dos métodos, mas continuam sendo um cenário controlado. A potência é uma estimativa por software, obtida dos sensores internos do processador, e os resultados valem para o computador e as condições usadas. Apresente os achados como um ensaio controlado com dados fictícios.
O treinamento do modelo é feito a cada execução, para manter o protótipo autocontido, e seu custo é registrado separadamente da classificação, pois uma implantação poderia treinar o modelo antes do uso.
Este protótipo é um programa de console para experimentação em lote, não uma API de produção.
