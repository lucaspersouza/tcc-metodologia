using System.Globalization;
using System.Text;

namespace TccGreenAi;

/// <summary>
/// Gera as bases sintéticas do experimento. Há três conjuntos de frases, mantidos separados:
/// treinamento (100 frases, só para o Naive Bayes), desenvolvimento (20 frases usadas para escrever as
/// palavras-chave das regras) e teste (120 frases, usadas apenas para avaliar os dois classificadores).
/// </summary>
public static class MessageGenerator
{
    /// <summary>Conjunto de teste: 30 frases por categoria. Nenhuma frase aparece no treino nem no desenvolvimento.</summary>
    private static readonly (MessageCategory Category, string[] Phrases)[] TestPhrases =
    [
        (MessageCategory.Suporte,
        [
            "O sistema apresentou um erro na hora de salvar o relatório.",
            "Não estou conseguindo acessar o painel desde ontem.",
            "Tem um bug na tela de configurações que impede de salvar as alterações.",
            "O app travou de novo quando abri o histórico.",
            "Estou com um problema para abrir os arquivos que enviei.",
            "Podem me dar uma ajuda com a importação dos contatos?",
            "A plataforma parou de funcionar no meio da minha apresentação.",
            "Houve uma falha ao enviar o formulário e perdi o que digitei.",
            "Quando tento acessar pelo celular, a página fica em branco.",
            "Apareceu um erro estranho ao exportar a planilha.",
            "Preciso de ajuda para recuperar minha senha.",
            "O programa travou assim que cliquei em atualizar.",
            "A impressão dos documentos está com problema desde a atualização.",
            "Aconteceu uma falha na sincronização com o meu calendário.",
            "O painel parou de funcionar para todos os usuários do meu setor.",
            "Não consigo entrar na minha conta, a senha é recusada.",
            "A tela congela toda vez que abro o módulo de relatórios.",
            "O aplicativo fecha sozinho logo depois de iniciar.",
            "As notificações pararam de chegar no meu e-mail.",
            "O sistema está muito lento e as páginas demoram a carregar.",
            "Meu login foi bloqueado e não recebo o código de verificação.",
            "Os arquivos que subi não aparecem na minha área.",
            "Está dando tela branca quando clico no botão de salvar.",
            "Depois que fiz o pagamento da assinatura, o sistema parou de funcionar para mim.",
            "Tentei comprar um módulo novo, mas deu erro na tela de confirmação.",
            "Não consigo acessar a área de pagamento, aparece só uma tela de erro.",
            "Depois da última atualização do plano, o sistema parou de funcionar.",
            "Quando clico para ver o plano contratado, a página trava.",
            "A tela de pagamento carrega e depois fica congelada.",
            "A consulta de vencimento aparece em branco no meu painel."
        ]),
        (MessageCategory.Vendas,
        [
            "Qual é o preço da versão para equipes pequenas?",
            "Gostaria de comprar acesso para mais cinco colaboradores.",
            "Podem me enviar um orçamento para a implantação na nossa filial?",
            "Quero contratar o serviço para a minha clínica.",
            "Vocês vendem licença individual ou só pacote corporativo?",
            "Existe desconto para quem fecha por um ano inteiro?",
            "Qual plano é mais indicado para uma loja pequena?",
            "Estou comparando o preço de vocês com o de outro fornecedor.",
            "Queremos comprar o sistema ainda neste mês.",
            "Preciso de um orçamento detalhado para apresentar à diretoria.",
            "Como funciona para contratar o suporte premium?",
            "Vocês oferecem licença temporária para testes?",
            "Tem algum desconto para ONGs?",
            "Quanto custa o plano que inclui relatórios avançados?",
            "Gostaria de saber o preço para atender duzentos usuários.",
            "Quais são os valores cobrados por usuário adicional?",
            "Tenho interesse em conhecer as opções de assinatura para minha empresa.",
            "Vocês fazem demonstração do produto antes de fechar negócio?",
            "Gostaria de saber quanto custa incluir mais dez usuários.",
            "Estamos avaliando adquirir a solução para três unidades.",
            "Há alguma promoção para novos clientes?",
            "Preciso de uma proposta comercial para enviar ao meu sócio.",
            "Gostaria de fechar um contrato de dois anos com vocês.",
            "Quero comprar o plano anual, mas antes preciso saber se aceitam boleto.",
            "Qual o preço do plano e como fica a fatura mensal?",
            "Queremos contratar vocês, mas tivemos um problema na demonstração.",
            "Gostaria de um orçamento, e antes preciso de ajuda para ver as opções.",
            "Queremos fechar com vocês; existe algum problema em atender a nossa filial em outra cidade?",
            "Preciso de ajuda para escolher entre as opções de assinatura.",
            "Vocês conseguem acessar o nosso sistema atual para fazer a migração, se fecharmos?"
        ]),
        (MessageCategory.Financeiro,
        [
            "Não recebi o boleto deste mês por e-mail.",
            "Minha fatura veio com um valor maior do que o combinado.",
            "O pagamento feito na sexta ainda não foi reconhecido pelo banco.",
            "Houve uma cobrança duplicada no meu cartão.",
            "Preciso que a nota fiscal saia com o CNPJ da minha empresa.",
            "Qual é a data de vencimento da próxima parcela?",
            "A mensalidade foi cobrada com um valor diferente do contrato.",
            "Gostaria de trocar o cartão usado no pagamento.",
            "Quero antecipar a quitação da fatura em aberto.",
            "O vencimento do boleto cai em feriado, posso pagar no dia seguinte?",
            "Preciso de um comprovante de pagamento para o meu contador.",
            "Vi uma cobrança que eu não reconheço no extrato.",
            "Como faço para alterar a data de vencimento da mensalidade?",
            "Recebi a fatura de novo mesmo depois de ter quitado.",
            "A nota fiscal está com o nome errado.",
            "Paguei ontem e o banco ainda não compensou.",
            "Quero parcelar o valor que está em atraso.",
            "Fui cobrado duas vezes pelo mesmo serviço.",
            "Preciso do comprovante dos últimos três meses para o meu contador.",
            "Gostaria de receber o recibo do que já foi quitado.",
            "O valor debitado no meu cartão não bate com o que eu esperava.",
            "Quero mudar a forma de pagar para débito automático.",
            "Estou com parcelas em atraso e quero regularizar.",
            "A fatura veio com o plano errado, preciso corrigir.",
            "Gostaria de saber se o desconto que me prometeram já entrou na fatura.",
            "Depois do pagamento do plano, a cobrança continuou aparecendo.",
            "Preciso de ajuda para entender a cobrança que apareceu no meu cartão.",
            "Houve um problema na compensação do valor que paguei.",
            "O desconto saiu errado no valor cobrado do meu cartão.",
            "Preciso de ajuda com o reembolso do valor que foi debitado duas vezes."
        ]),
        (MessageCategory.Outros,
        [
            "Vocês têm escritório em Curitiba?",
            "Existe um telefone para falar com um atendente?",
            "Quero atualizar o nome do responsável pela minha conta.",
            "Onde posso ler os comunicados mais recentes da empresa?",
            "Vocês estão contratando desenvolvedores?",
            "Qual é o e-mail para enviar currículo?",
            "Gostaria de participar do programa de parceiros.",
            "Vocês realizam eventos presenciais ou só online?",
            "Quero deixar um elogio para a equipe que me atendeu.",
            "Como faço para mudar o idioma da interface?",
            "Posso trazer um colega para a próxima reunião de apresentação?",
            "Vocês possuem algum certificado de segurança da informação?",
            "Qual o prazo para resposta quando envio uma dúvida geral?",
            "Gostaria de sugerir um novo recurso para o aplicativo.",
            "Onde encontro o manual de utilização do produto?",
            "Vocês têm uma página com perguntas frequentes?",
            "Quero saber se há um grupo de usuários onde posso trocar experiências.",
            "Preciso atualizar meu endereço de entrega de correspondências.",
            "Como faço para alterar o e-mail de contato da minha conta?",
            "Vocês publicam algum relatório de sustentabilidade?",
            "Que horas termina o atendimento por chat nos sábados?",
            "A empresa participa de feiras de tecnologia?",
            "Gostaria de saber quem é o responsável pela área de parcerias.",
            "Vocês oferecem cursos para quem está começando?",
            "Qual é o plano de trabalho da equipe para o próximo semestre?",
            "Estou com um problema de agenda e preciso remarcar a reunião de apresentação.",
            "Obrigado pela ajuda que a equipe me deu ontem!",
            "Posso comprar um café na recepção quando visitar o escritório de vocês?",
            "Vocês têm desconto de estacionamento para visitantes no prédio?",
            "Onde consigo acessar a vaga de estágio anunciada no site?"
        ])
    ];

    /// <summary>Conjunto de desenvolvimento: as 20 frases da primeira versão, usadas para escrever as palavras-chave das regras.</summary>
    private static readonly (MessageCategory Category, string[] Phrases)[] DevelopmentPhrases =
    [
        (MessageCategory.Suporte,
        [
            "O aplicativo travou quando tentei entrar.",
            "Estou com erro ao acessar minha conta.",
            "O sistema parou de funcionar depois da atualização.",
            "Preciso de ajuda para resolver uma falha no cadastro.",
            "A tela inicial está apresentando um bug."
        ]),
        (MessageCategory.Vendas,
        [
            "Qual é o preço do plano empresarial?",
            "Quero comprar uma licença para minha equipe.",
            "Gostaria de receber um orçamento do produto.",
            "Como faço para contratar uma assinatura?",
            "Vocês têm desconto para adquirir mais licenças?"
        ]),
        (MessageCategory.Financeiro,
        [
            "Preciso da segunda via do boleto.",
            "Já paguei a fatura, mas o pagamento não foi confirmado.",
            "Como atualizo os dados de cobrança?",
            "A nota fiscal ainda não chegou.",
            "Quero consultar o vencimento da minha mensalidade."
        ]),
        (MessageCategory.Outros,
        [
            "Qual é o horário de atendimento da empresa?",
            "Quero atualizar meu endereço de contato.",
            "Onde encontro as novidades do produto?",
            "Gostaria de fazer uma sugestão para o site.",
            "Como posso falar com a equipe de atendimento?"
        ])
    ];

    /// <summary>Corpus de treinamento do Naive Bayes: 25 frases por categoria, todas diferentes das do teste.</summary>
    private static readonly (MessageCategory Category, string[] Phrases)[] TrainingPhrases =
    [
        (MessageCategory.Suporte,
        [
            "Não consigo entrar no aplicativo, aparece uma falha.",
            "A página congelou e não responde aos comandos.",
            "O acesso à conta está bloqueado.",
            "Depois da atualização o programa deixou de abrir.",
            "Preciso de assistência com a configuração do sistema.",
            "A tela apresenta uma mensagem inesperada.",
            "O aplicativo travou durante o acesso.",
            "Recebi um erro e preciso de ajuda técnica.",
            "Meu usuário foi bloqueado depois de várias tentativas de entrada.",
            "O site demora muito para abrir e às vezes nem carrega.",
            "Recebi um aviso de falha ao anexar um documento no chamado.",
            "A aplicação fechou sozinha duas vezes hoje.",
            "Não chega o e-mail para redefinir a senha.",
            "Os botões do menu não respondem quando clico neles.",
            "Depois da atualização o aplicativo ficou muito lento.",
            "Tem algum defeito no módulo de agenda, os horários aparecem trocados.",
            "Não consigo fazer login com o meu e-mail corporativo.",
            "O sistema travou na hora de gerar o arquivo e perdi o trabalho.",
            "Preciso de suporte técnico para configurar a impressora na rede.",
            "A página de contatos aparece em branco para mim.",
            "O aplicativo no celular não sincroniza com a versão do computador.",
            "Surgiu um bug estranho nos gráficos do painel principal.",
            "Pode me orientar sobre como instalar o programa novamente?",
            "A busca do sistema não encontra os registros que eu cadastrei.",
            "Os dados que salvei ontem desapareceram do sistema."
        ]),
        (MessageCategory.Vendas,
        [
            "Quais opções de assinatura estão disponíveis para empresas?",
            "Tenho interesse em adquirir o serviço.",
            "Poderiam enviar uma proposta comercial?",
            "Quero conhecer os valores dos pacotes.",
            "É possível incluir novos usuários no contrato?",
            "Gostaria de negociar a compra de uma solução.",
            "Quero comprar uma licença e consultar o preço.",
            "Solicito orçamento e desconto para adquirir o plano.",
            "Como contratar licenças adicionais para a equipe?",
            "Preciso de informações sobre os pacotes disponíveis para empresas médias.",
            "Qual o valor mensal para cinquenta usuários?",
            "Vocês oferecem período de teste gratuito?",
            "Gostaria de agendar uma apresentação comercial com um consultor.",
            "Quero adquirir o produto para o meu escritório de contabilidade.",
            "Existe alguma condição especial para contratos de longo prazo?",
            "Como faço para ampliar o número de usuários do meu plano?",
            "Qual a diferença entre a versão básica e a profissional?",
            "Estou interessado no serviço, podem entrar em contato comigo?",
            "Vocês atendem empresas de pequeno porte com preço reduzido?",
            "Quais formas de assinatura vocês disponibilizam?",
            "Gostaria de uma cotação para instalar em duas lojas.",
            "Posso migrar do meu fornecedor atual para vocês sem perder os dados?",
            "Queremos fechar parceria para revender o produto.",
            "Qual o investimento necessário para começar a usar a ferramenta?",
            "Há condições diferenciadas para instituições de ensino?"
        ]),
        (MessageCategory.Financeiro,
        [
            "Meu pagamento ainda consta como pendente.",
            "Onde posso obter o documento fiscal da compra?",
            "A cobrança veio com um valor diferente.",
            "Qual a data limite para quitar a mensalidade?",
            "Preciso alterar a forma de pagamento cadastrada.",
            "Solicito o comprovante referente à última parcela.",
            "Preciso emitir a segunda via do boleto bancário.",
            "A fatura e a cobrança estão incorretas.",
            "A nota fiscal do pagamento não foi enviada.",
            "Quando vence a mensalidade da assinatura?",
            "Preciso do comprovante de quitação para apresentar na minha empresa.",
            "Apareceram dois lançamentos iguais no extrato do cartão.",
            "O valor da parcela veio diferente do que consta no contrato.",
            "Quero atualizar o cartão de crédito cadastrado para os pagamentos.",
            "Poderiam reenviar o link para pagar a conta deste mês?",
            "Minha cobrança foi feita depois do cancelamento do serviço.",
            "Preciso do recibo do pagamento realizado na semana passada.",
            "Quero mudar a data em que o valor é debitado todo mês.",
            "A fatura de outubro não apareceu na minha área do cliente.",
            "Já quitei o débito, mas o sistema continua mostrando pendência.",
            "Como solicito reembolso de um valor cobrado indevidamente?",
            "Gostaria de negociar o parcelamento das mensalidades atrasadas.",
            "Emitiram a nota fiscal com a razão social errada.",
            "Até que dia posso pagar sem multa?",
            "O banco recusou a transação e não sei se houve cobrança."
        ]),
        (MessageCategory.Outros,
        [
            "Qual é o horário de funcionamento da equipe?",
            "Quero sugerir uma melhoria para o site.",
            "Onde encontro informações sobre a empresa?",
            "Gostaria de alterar meu telefone de contato.",
            "Vocês oferecem treinamento para usuários?",
            "Como faço para acompanhar os comunicados?",
            "Quero saber os horários de atendimento ao público.",
            "Tenho uma sugestão para melhorar a página.",
            "Qual é o endereço da sede de vocês?",
            "Gostaria de conhecer a história da empresa.",
            "Vocês têm perfil nas redes sociais?",
            "Preciso alterar o CEP do meu cadastro.",
            "Como faço para assinar a newsletter da empresa?",
            "Gostaria de enviar um elogio sobre o atendimento de ontem.",
            "Vocês realizam treinamentos em vídeo para novos usuários?",
            "Posso visitar o escritório de vocês para conhecer a equipe?",
            "Qual é o horário do plantão aos fins de semana?",
            "Onde acompanho as novidades e as próximas atualizações do produto?",
            "Quero indicar um amigo para trabalhar com vocês.",
            "Existe um blog com dicas de uso?",
            "Como posso alterar meu nome de exibição?",
            "Vocês têm vagas para estágio na área de tecnologia?",
            "Gostaria de saber se a empresa tem política de privacidade publicada.",
            "Qual é o canal oficial para enviar uma reclamação sobre o atendimento?",
            "Posso participar de um webinar para conhecer melhor a empresa?"
        ])
    ];

    /// <summary>
    /// Gera a base de avaliação. Cada frase de teste aparece o mesmo número de vezes (count / 120);
    /// se count não for múltiplo de 120, as frases restantes ficam com uma ocorrência a mais.
    /// A ordem final é embaralhada de forma reproduzível pela semente.
    /// </summary>
    public static List<Message> Generate(int count, int seed = 42)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "A quantidade deve ser maior que zero.");

        EnsureSetsAreDisjoint();

        var phrases = TestPhrases
            .SelectMany(group => group.Phrases.Select(text => (group.Category, Text: text)))
            .ToArray();

        var selected = new (MessageCategory Category, string Text)[count];
        for (var i = 0; i < count; i++)
            selected[i] = phrases[i % phrases.Length];

        var random = new Random(seed);
        for (var i = count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (selected[i], selected[j]) = (selected[j], selected[i]);
        }

        var messages = new List<Message>(count);
        for (var i = 0; i < count; i++)
            messages.Add(new Message(i + 1, selected[i].Text, selected[i].Category));

        return messages;
    }

    /// <summary>Corpus sintético, separado do conjunto de teste, usado para treinar o Naive Bayes.</summary>
    public static List<Message> GenerateTrainingSet()
    {
        EnsureSetsAreDisjoint();

        var messages = new List<Message>();
        var id = 1;
        foreach (var (category, phrases) in TrainingPhrases)
            foreach (var text in phrases)
                messages.Add(new Message(id++, text, category));

        return messages;
    }

    /// <summary>Garante que não há frases repetidas dentro do teste e que treino, desenvolvimento e teste não se sobrepõem.</summary>
    private static void EnsureSetsAreDisjoint()
    {
        var test = Flatten(TestPhrases);
        var development = Flatten(DevelopmentPhrases);
        var training = Flatten(TrainingPhrases);

        if (test.Distinct().Count() != test.Count)
            throw new InvalidOperationException("O conjunto de teste contém frases repetidas.");

        var overlap = test.Intersect(training).Concat(test.Intersect(development)).FirstOrDefault();
        if (overlap is not null)
            throw new InvalidOperationException($"A frase aparece em mais de um conjunto: {overlap}");
    }

    private static List<string> Flatten((MessageCategory Category, string[] Phrases)[] groups) =>
        groups.SelectMany(group => group.Phrases).Select(NormalizeForComparison).ToList();

    private static string NormalizeForComparison(string value)
    {
        var decomposed = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}