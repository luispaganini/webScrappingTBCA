using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text.RegularExpressions;
using WebScrapping_Foods.Entities;
using WebScrapping_Foods.Models;

class Program {
    static async Task Main(string[] args) {
        var optionsBuilder = new DbContextOptionsBuilder<Balanced_LifeContext>();
        optionsBuilder.UseSqlServer("Data Source=localhost\\MSSQLSERVER02;Initial Catalog=Balanced_Life;Integrated Security=True;Encrypt=True;TrustServerCertificate=YES");

        int page = 1;
        bool finished = false;
        do {
            var htmlDocument = await GetHtmlDocument($"https://www.tbca.net.br/base-dados/composicao_alimentos.php?pagina={page}&atuald=1");
            // Seleciona a tabela com a classe "tbca"
            var tabela = htmlDocument
                .DocumentNode.SelectSingleNode("//tbody");

            using ( var dbContext = new Balanced_LifeContext(optionsBuilder.Options) ) {
                var groups = await dbContext.GetAllGroupsAsync();

                if ( tabela != null ) {
                    // Itera sobre as linhas da tabela, ignorando o cabeçalho
                    var linhas = tabela.SelectNodes(".//tr[position()>1]");

                    if ( linhas == null ) {
                        finished = true;
                        break;
                    }

                    foreach ( var linha in linhas ) {
                        var celulas = linha.SelectNodes(".//td");

                        var group = groups.FirstOrDefault(g => g.Name.ToUpper() == celulas[3].InnerText.Trim().ToUpper());

                        Food food = new Food() {
                            Name = celulas[1].InnerText.Trim(),
                            Brand = celulas[4].InnerText.Trim(),
                            IdFoodGroup = group != null ? group.Id : 0,
                            ReferenceTable = "TBCA"
                        };

                        dbContext.Add(food);

                        await dbContext.SaveChangesAsync();

                        await GetNutritionInfo(celulas[0].InnerText.Trim(), optionsBuilder, food.Id);
                        Console.WriteLine(food.Name);
                    }
                    page++;
                } else {
                    Console.WriteLine("Tabela não encontrada.");
                    page++;
                }
            }

        } while ( !finished );
    }
    public static async Task<HtmlDocument> GetHtmlDocument(string url) {
        var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url);

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        return htmlDocument;
    }

    public static async Task<FoodNutritionInfo> GetNutritionInfo(string id, DbContextOptionsBuilder<Balanced_LifeContext> builder, long idFood) {
        var document = await GetHtmlDocument($"https://www.tbca.net.br/base-dados/int_composicao_alimentos.php?cod_produto={id}");

        var tabela = document
            .DocumentNode.SelectSingleNode("//tbody");
        using ( var dbContext = new Balanced_LifeContext(builder.Options) ) {
            if ( tabela != null ) {
                // Itera sobre as linhas da tabela, ignorando o cabeçalho
                var linhas = tabela.SelectNodes(".//tr[position()>1]");
                if (linhas != null) {
                    foreach ( var linha in linhas ) {
                        var celulas = linha.SelectNodes(".//td");


                        var nutritionalComposition = await dbContext.GetCompositionByNameAsync(celulas[0].InnerText.Trim());
                        var unitMeasurement = await dbContext.GetUnitMeasurementByNameAsync(celulas[1].InnerText.Trim());

                        var numberFormatInfo = new NumberFormatInfo {
                            NumberDecimalSeparator = ",",
                            NumberGroupSeparator = "."
                        };
                        float quantidade;
                        if ( !float.TryParse(celulas[2].InnerText.Trim(), NumberStyles.Float, numberFormatInfo, out quantidade) )
                            quantidade = 0.0f;

                        var nutritionInfo = new FoodNutritionInfo() {
                            IdFood = idFood,
                            IdNutritionalComposition = nutritionalComposition != null ? nutritionalComposition.Id : 0,
                            IdUnitMeasurement = unitMeasurement != null ? unitMeasurement.Id : 0,
                            Quantity = quantidade
                        };

                        dbContext.Add(nutritionInfo);

                        await dbContext.SaveChangesAsync();
                    }
                }
            } else {
                Console.WriteLine("Tabela não encontrada.");
            }
        }
         return new FoodNutritionInfo();
    }

}

