using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace api_artur.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaceMarksController : ControllerBase
    {
        // Filtro de dados para valida��o de pr�-sele��o
        private readonly string[] clientesValidos = { "GRADE", "MARAT�", "MOURA" };
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
                    return BadRequest("O valor informado para 'CLIENTE' n�o esta na pr�-sele��o de valores.");
                }

                if (!string.IsNullOrEmpty(situacao) && !situacoesValidas.Contains(situacao))
                {
                    return BadRequest("O valor informado para 'SITUA��O' n�o esta na pr�-sele��o de valores.");
                }

                if (!string.IsNullOrEmpty(bairro) && !bairrosValidos.Contains(bairro))
                {
                    return BadRequest("O valor informado para 'BAIRRO' n�o esta na pr�-sele��o de valores.");
                }

                var filePath = "DIRECIONADORES1.kml";
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Arquivo KML n�o encontrado.");
                }

                XNamespace ns = "http://www.opengis.net/kml/2.2";
                var kmlDocument = XDocument.Load(filePath);

                // Filtragem com base nos par�metros
                var filteredPlacemarks = kmlDocument.Descendants(ns + "Placemark")
                    .Where(pm =>
                        (string.IsNullOrEmpty(cliente) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "CLIENTE" && d.Element(ns + "value")?.Value == cliente)) &&
                        (string.IsNullOrEmpty(situacao) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "SITUA��O" && d.Element(ns + "value")?.Value == situacao)) &&
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
                    return BadRequest("N�o foi poss�vel gerar o arquivo .kml. Informe valores v�lidos!");
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

                // Salvar o novo arquivo .KML em mem�ria
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
                    return BadRequest("O valor informado para 'CLIENTE' n�o est� na pr�-sele��o de valores.");
                }

                if (!string.IsNullOrEmpty(situacao) && !situacoesValidas.Contains(situacao))
                {
                    return BadRequest("O valor informado para 'SITUA��O' n�o est� na pr�-sele��o de valores.");
                }

                if (!string.IsNullOrEmpty(bairro) && !bairrosValidos.Contains(bairro))
                {
                    return BadRequest("O valor informado para 'BAIRRO' n�o est� na pr�-sele��o de valores.");
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
                    return NotFound("Arquivo KML n�o encontrado.");
                }

                XNamespace ns = "http://www.opengis.net/kml/2.2";
                var kmlDocument = XDocument.Load(filePath);

                var filteredPlacemarks = kmlDocument.Descendants(ns + "Placemark")
                    .Where(pm =>
                        (string.IsNullOrEmpty(cliente) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "CLIENTE" && d.Element(ns + "value")?.Value == cliente)) &&
                        (string.IsNullOrEmpty(situacao) ||
                            pm.Descendants(ns + "Data").Any(d => (string)d.Attribute("name") == "SITUA��O" && d.Element(ns + "value")?.Value == situacao)) &&
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
                    return BadRequest("N�o foi poss�vel encontrar nenhum dado com os filtros informados.");
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