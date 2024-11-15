using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace api_artur.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaceMarksController : ControllerBase
    {
        // Filtro de dados para validação de pré-seleção
        private readonly string[] clientesValidos = { "GRADE", "MARATÁ", "MOURA" };
        private readonly string[] situacoesValidas = { "", "DISPONIVEL", "PRA ONDE FOI REVEMAR????" };
        private readonly string[] bairrosValidos = { "JARDINS", "TREZE DE JULHO", "GRAGERU" };

        [HttpPost("/api/placemarks/export")]
        public IActionResult FilterKml(
            [FromQuery] string? cliente,
            [FromQuery] string? situacao,
            [FromQuery] string? bairro,
            [FromQuery] string? referencia,
            [FromQuery] string? ruaCruzamento)
        {
            try
            {
                if (!string.IsNullOrEmpty(referencia) && referencia.Length < 3)
                {
                    return BadRequest("O campo 'REFERENCIA' deve conter pelo menos 3 caracteres.");
                }

                if (!string.IsNullOrEmpty(ruaCruzamento) && ruaCruzamento.Length < 3)
                {
                    return BadRequest("O campo 'RUA/CRUZAMENTO' deve conter pelo menos 3 caracteres.");
                }

                if (!string.IsNullOrEmpty(cliente) && !clientesValidos.Contains(cliente))
                {
                    return BadRequest("O valor informado para 'CLIENTE' não esta na pré-seleção de valores.");
                }

                if (!string.IsNullOrEmpty(situacao) && !situacoesValidas.Contains(situacao))
                {
                    return BadRequest("O valor informado para 'SITUAÇÃO' não esta na pré-seleção de valores.");
                }

                if (!string.IsNullOrEmpty(bairro) && !bairrosValidos.Contains(bairro))
                {
                    return BadRequest("O valor informado para 'BAIRRO' não esta na pré-seleção de valores.");
                }

                var filePath = "DIRECIONADORES1.kml";
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Arquivo KML não encontrado.");
                }

                XNamespace ns = "http://www.opengis.net/kml/2.2";
                var kmlDocument = XDocument.Load(filePath);

                // Filtragem com base nos parâmetros
                var filteredPlacemarks = kmlDocument.Descendants(ns + "Placemark")
                    .Where(pm =>
                        (string.IsNullOrEmpty(cliente) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "CLIENTE" && d.Element(ns + "value")?.Value == cliente)) &&
                        (string.IsNullOrEmpty(situacao) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "SITUAÇÃO" && d.Element(ns + "value")?.Value == situacao)) &&
                        (string.IsNullOrEmpty(bairro) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "BAIRRO" && d.Element(ns + "value")?.Value == bairro)) &&
                        (string.IsNullOrEmpty(referencia) ||
                            pm.Descendants(ns + "Data").Any(d =>
                                (string)d.Attribute("name") == "REFERENCIA" &&
                                d.Element(ns + "value")?.Value?.Contains(referencia) == true)) &&
                        (string.IsNullOrEmpty(ruaCruzamento) ||
                            pm.Descendants(ns + "Data").Any(d =>
                                (string)d.Attribute("name") == "RUA/CRUZAMENTO" &&
                                d.Element(ns + "value")?.Value?.Contains(ruaCruzamento) == true))
                    ).ToList();

                if (filteredPlacemarks.Count() == 0)
                {
                    return BadRequest("Não foi possível gerar o arquivo .kml. Informe valores válidos!");
                }

                // Criar um novo documento .KML com os placemarks filtrados
                var newKmlDocument = new XDocument(
                    new XElement(ns + "kml",
                        new XElement(ns + "Document",
                            new XElement(ns + "name", "Filtered Placemarks"),
                            filteredPlacemarks
                        )
                    )
                );

                // Salvar o novo arquivo .KML em memória
                var memoryStream = new MemoryStream();
                newKmlDocument.Save(memoryStream);
                memoryStream.Position = 0;

                // Retornar o novo arquivo .KML como resposta
                return File(memoryStream, "application/vnd.google-earth.kml+xml", "ArquivoFiltrado.kml");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/api/placemarks")]
        public IActionResult GetFilteredPlacemarks(
            [FromQuery] string? cliente,
            [FromQuery] string? situacao,
            [FromQuery] string? bairro,
            [FromQuery] string? referencia,
            [FromQuery] string? ruaCruzamento)
        {

            try
            {
                if (!string.IsNullOrEmpty(cliente) && !clientesValidos.Contains(cliente))
                {
                    return BadRequest("O valor informado para 'CLIENTE' não está na pré-seleção de valores.");
                }

                if (!string.IsNullOrEmpty(situacao) && !situacoesValidas.Contains(situacao))
                {
                    return BadRequest("O valor informado para 'SITUAÇÃO' não está na pré-seleção de valores.");
                }

                if (!string.IsNullOrEmpty(bairro) && !bairrosValidos.Contains(bairro))
                {
                    return BadRequest("O valor informado para 'BAIRRO' não está na pré-seleção de valores.");
                }

                if (!string.IsNullOrEmpty(referencia) && referencia.Length < 3)
                {
                    return BadRequest("O campo 'REFERENCIA' deve conter pelo menos 3 caracteres.");
                }

                if (!string.IsNullOrEmpty(ruaCruzamento) && ruaCruzamento.Length < 3)
                {
                    return BadRequest("O campo 'RUA/CRUZAMENTO' deve conter pelo menos 3 caracteres.");
                }

                var filePath = "DIRECIONADORES1.kml";
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Arquivo KML não encontrado.");
                }

                XNamespace ns = "http://www.opengis.net/kml/2.2";
                var kmlDocument = XDocument.Load(filePath);

                var filteredPlacemarks = kmlDocument.Descendants(ns + "Placemark")
                    .Where(pm =>
                        (string.IsNullOrEmpty(cliente) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "CLIENTE" && d.Element(ns + "value")?.Value == cliente)) &&
                        (string.IsNullOrEmpty(situacao) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "SITUAÇÃO" && d.Element(ns + "value")?.Value == situacao)) &&
                        (string.IsNullOrEmpty(bairro) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "BAIRRO" && d.Element(ns + "value")?.Value == bairro)) &&
                        (string.IsNullOrEmpty(referencia) ||
                            pm.Descendants(ns + "Data").Any(d =>
                                (string)d.Attribute("name") == "REFERENCIA" &&
                                d.Element(ns + "value")?.Value?.Contains(referencia) == true)) &&
                        (string.IsNullOrEmpty(ruaCruzamento) ||
                            pm.Descendants(ns + "Data").Any(d =>
                                (string)d.Attribute("name") == "RUA/CRUZAMENTO" &&
                                d.Element(ns + "value")?.Value?.Contains(ruaCruzamento) == true))
                    ).ToList();

                if (filteredPlacemarks.Count == 0)
                {
                    return BadRequest("Não foi possível encontrar nenhum dado com os filtros informados.");
                }

                // Retornar os placemarks filtrados no formato JSON
                string placemarks = JsonConvert.SerializeObject(filteredPlacemarks);

                return Ok(placemarks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("/api/placemarks/filters")]
        public IActionResult GetAvailableFilters()
        {
            try
            {
                var filters = new
                {
                    Clientes = clientesValidos,
                    Situacoes = situacoesValidas,
                    Bairros = bairrosValidos
                };

                return Ok(filters);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}