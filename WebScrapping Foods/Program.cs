using HtmlAgilityPack;

class Program {
    static async Task Main(string[] args) {

        var htmlDocument = await GetHtmlDocument("https://www.tbca.net.br/base-dados/composicao_alimentos.php?pagina=1&atuald=1");
        // Seleciona a tabela com a classe "tbca"
        var tabela = htmlDocument
            .DocumentNode.SelectSingleNode("//tbody");

        if ( tabela != null ) {
            // Itera sobre as linhas da tabela, ignorando o cabeçalho
            var linhas = tabela.SelectNodes(".//tr[position()>1]");

            foreach ( var linha in linhas ) {
                // Seleciona as células da linha
                var celulas = linha.SelectNodes(".//td");

                // Extrai os dados das células
                var nome = celulas[0].InnerText.Trim();
                var calorias = celulas[1].InnerText.Trim();
                var proteina = celulas[2].InnerText.Trim();
                var gordura = celulas[3].InnerText.Trim();
                var carboidratos = celulas[4].InnerText.Trim();

                // Exibe os dados
                Console.WriteLine($"ID: {nome}, Nome: {calorias}, Nome Cientifico: {proteina}, Grupo: {gordura}, Marca: {carboidratos}");
            }
        } else {
            Console.WriteLine("Tabela não encontrada.");
        }
    }
    public static async Task<HtmlDocument> GetHtmlDocument(string url) {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        return htmlDocument;
    }

}

