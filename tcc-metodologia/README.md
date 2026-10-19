# Experimento do TCC: classificação com e sem IA

O programa executa **um método por vez**, para você iniciar o registro no HWiNFO e medir cada sistema separadamente. Ambos recebem a mesma base de mensagens fictícias quando usados com a mesma quantidade e semente.

- `rules`: sistema sem IA, classifica por regras de palavras-chave.
- `ai`: sistema com IA local, usa Naive Bayes treinado com um corpus sintético separado.

Não são usados dados reais, serviços externos ou APIs. As mensagens e os rótulos são fictícios.

## Como gravar uma execução de cada sistema

1. Inicie o registro dos sensores no HWiNFO e use a mesma configuração e intervalo de amostragem nas duas execuções.
2. Rode o sistema sem IA:

   ```bash
   dotnet run --no-restore -- --method rules --count 5000 --seed 42 --repetitions 200
   ```

3. Salve o registro do HWiNFO com um nome que identifique `rules`.
4. Rode o sistema com IA local, usando os mesmos parâmetros:

   ```bash
   dotnet run --no-restore -- --method ai --count 5000 --seed 42 --repetitions 200
   ```

5. Salve o segundo registro do HWiNFO com um nome que identifique `ai`.

O console imprime marcadores com horário para início do experimento, treinamento e classificação. Use `CLASSIFICACAO_INICIO` e `CLASSIFICACAO_FIM` para delimitar a comparação da classificação no HWiNFO. O treinamento do modelo de IA é marcado separadamente. Para comparar o custo total de inicialização, treinamento e classificação, considere o intervalo inteiro entre `EXPERIMENTO_INICIO` e `EXPERIMENTO_FIM`.

O padrão é gerar 5.000 mensagens, semente 42 e 20 repetições. `--repetitions 200` prolonga as fases para facilitar a coleta de sensores. Use a mesma quantidade e o mesmo número de repetições nos dois métodos. O sistema gera os arquivos separadamente:

- `resultados/rules-mensagens.csv` e `resultados/rules-metricas.csv`
- `resultados/ai-mensagens.csv` e `resultados/ai-metricas.csv`

É possível mudar o tamanho do lote, semente, repetições e caminhos:

```bash
dotnet run --no-restore -- --method ai --count 5000 --seed 42 --repetitions 200 --output resultados/ai-mensagens.csv --metrics resultados/ai-metricas.csv
```

## Métricas

O CSV de métricas registra acertos, erros, acurácia sobre os rótulos fictícios, vazão, tempo de classificação, tempo de CPU do processo e amostras de memória antes/depois. O treinamento do Naive Bayes aparece separadamente. O HWiNFO deve registrar os sensores de hardware, como uso de CPU, frequência, temperatura e potência, quando disponíveis. O programa **não controla nem exporta dados do HWiNFO**; os registros de sensores são arquivos separados.

Se comparar energia, use o mesmo intervalo e carga nos dois métodos; informe a quantidade total de classificações (mensagens × repetições) e normalize a energia pelo número de classificações, se esse for o indicador escolhido. Temperatura e potência dependem do computador, da ventilação, do plano de energia e de processos em segundo plano; documente o ambiente e faça execuções repetidas. Alterne a ordem dos métodos entre repetições do experimento para reduzir efeitos de aquecimento.

## Limites acadêmicos

A base e o corpus de treinamento são sintéticos. A acurácia mede somente esses exemplos; não demonstra desempenho em mensagens reais. As regras foram construídas para as frases de avaliação, então resultados altos do método sem IA não são evidência de generalização. Apresente os achados como um ensaio controlado com dados fictícios. O treinamento do modelo é feito em cada execução para manter o protótipo autocontido; reporte esse custo separado da inferência, pois uma implantação poderia treinar o modelo antes do uso.

Este protótipo é um programa de console para experimentação em lote, não uma API de produção.
