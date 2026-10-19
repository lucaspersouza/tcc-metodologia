namespace TccGreenAi;

/// <summary>Gera uma base sintética reproduzível, com rótulos conhecidos para avaliação.</summary>
public static class MessageGenerator
{
    private static readonly (MessageCategory Category, string[] Templates)[] Examples =
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

    public static List<Message> Generate(int count, int seed = 42)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "A quantidade deve ser maior que zero.");

        var random = new Random(seed);
        var messages = new List<Message>(count);

        for (var i = 0; i < count; i++)
        {
            var example = Examples[random.Next(Examples.Length)];
            var text = example.Templates[random.Next(example.Templates.Length)];
            messages.Add(new Message(i + 1, text, example.Category));
        }

        return messages;
    }

    /// <summary>Corpus sintético separado do lote de avaliação para treinar o modelo de aprendizado de máquina.</summary>
    public static List<Message> GenerateTrainingSet() =>
    [
        new(1, "Não consigo entrar no aplicativo, aparece uma falha.", MessageCategory.Suporte),
        new(2, "A página congelou e não responde aos comandos.", MessageCategory.Suporte),
        new(3, "O acesso à conta está bloqueado.", MessageCategory.Suporte),
        new(4, "Depois da atualização o programa deixou de abrir.", MessageCategory.Suporte),
        new(5, "Preciso de assistência com a configuração do sistema.", MessageCategory.Suporte),
        new(6, "A tela apresenta uma mensagem inesperada.", MessageCategory.Suporte),
        new(25, "O aplicativo travou durante o acesso.", MessageCategory.Suporte),
        new(26, "Recebi um erro e preciso de ajuda técnica.", MessageCategory.Suporte),
        new(7, "Quais opções de assinatura estão disponíveis para empresas?", MessageCategory.Vendas),
        new(8, "Tenho interesse em adquirir o serviço.", MessageCategory.Vendas),
        new(9, "Poderiam enviar uma proposta comercial?", MessageCategory.Vendas),
        new(10, "Quero conhecer os valores dos pacotes.", MessageCategory.Vendas),
        new(11, "É possível incluir novos usuários no contrato?", MessageCategory.Vendas),
        new(12, "Gostaria de negociar a compra de uma solução.", MessageCategory.Vendas),
        new(27, "Quero comprar uma licença e consultar o preço.", MessageCategory.Vendas),
        new(28, "Solicito orçamento e desconto para adquirir o plano.", MessageCategory.Vendas),
        new(29, "Como contratar licenças adicionais para a equipe?", MessageCategory.Vendas),
        new(13, "Meu pagamento ainda consta como pendente.", MessageCategory.Financeiro),
        new(14, "Onde posso obter o documento fiscal da compra?", MessageCategory.Financeiro),
        new(15, "A cobrança veio com um valor diferente.", MessageCategory.Financeiro),
        new(16, "Qual a data limite para quitar a mensalidade?", MessageCategory.Financeiro),
        new(17, "Preciso alterar a forma de pagamento cadastrada.", MessageCategory.Financeiro),
        new(18, "Solicito o comprovante referente à última parcela.", MessageCategory.Financeiro),
        new(30, "Preciso emitir a segunda via do boleto bancário.", MessageCategory.Financeiro),
        new(31, "A fatura e a cobrança estão incorretas.", MessageCategory.Financeiro),
        new(32, "A nota fiscal do pagamento não foi enviada.", MessageCategory.Financeiro),
        new(33, "Quando vence a mensalidade da assinatura?", MessageCategory.Financeiro),
        new(19, "Qual é o horário de funcionamento da equipe?", MessageCategory.Outros),
        new(20, "Quero sugerir uma melhoria para o site.", MessageCategory.Outros),
        new(21, "Onde encontro informações sobre a empresa?", MessageCategory.Outros),
        new(22, "Gostaria de alterar meu telefone de contato.", MessageCategory.Outros),
        new(23, "Vocês oferecem treinamento para usuários?", MessageCategory.Outros),
        new(24, "Como faço para acompanhar os comunicados?", MessageCategory.Outros),
        new(34, "Quero saber os horários de atendimento ao público.", MessageCategory.Outros),
        new(35, "Tenho uma sugestão para melhorar a página.", MessageCategory.Outros)
    ];
}
