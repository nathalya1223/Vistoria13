using Microsoft.AspNetCore.Mvc;
using Vistoria_projeto.Context;
using Vistoria_projeto.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using ImageMagick;

namespace Vistoria_projeto.Controllers
{
    public class ChecklistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ChecklistController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index() => RedirectToAction("Lista");

        // LISTA
        public IActionResult Lista(string status = "Todas")
        {
            var query = _context.ChecklistsVistorias.AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "Todas")
                query = query.Where(v => v.Status == status);

            var lista = query.OrderByDescending(v => v.Data).ToList();
            ViewBag.FiltroAtual = status;

            return View(lista);
        }

        // DETALHES
        public IActionResult Detalhes(int id)
        {
            var vistoria = _context.ChecklistsVistorias.FirstOrDefault(v => v.Id == id);
            if (vistoria == null) return NotFound();

            return View(vistoria);
        }

        public IActionResult Nova() => View();

        [HttpPost]
        public IActionResult Nova(ChecklistVistoria checklist)
        {
            return View("ChecklistCompleto", checklist);
        }

        // EXCLUIR
        [HttpGet]
        public async Task<IActionResult> Excluir(int id)
        {
            var vistoria = _context.ChecklistsVistorias.FirstOrDefault(v => v.Id == id);
            if (vistoria == null) return NotFound();

            if (!string.IsNullOrEmpty(vistoria.CaminhoFoto))
            {
                string caminho = Path.Combine(_env.WebRootPath, vistoria.CaminhoFoto.TrimStart('/'));
                if (System.IO.File.Exists(caminho))
                    System.IO.File.Delete(caminho);
            }

            _context.ChecklistsVistorias.Remove(vistoria);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Vistoria excluída!";
            return RedirectToAction("Lista");
        }

        // FORM COMPLETO
        public IActionResult ChecklistCompleto(ChecklistVistoria checklist)
        {
            if (checklist == null)
                checklist = new ChecklistVistoria();

            return View(checklist);
        }

        // SALVAR
        [HttpPost]
        public async Task<IActionResult> SalvarChecklistCompleto(ChecklistVistoria checklist, IFormFile fotoVistoria)
        {
            if (fotoVistoria != null)
            {
                string pasta = Path.Combine(_env.WebRootPath, "fotosVistorias");
                Directory.CreateDirectory(pasta);

                string nome = Guid.NewGuid() + Path.GetExtension(fotoVistoria.FileName);
                string caminho = Path.Combine(pasta, nome);

                using (var stream = new FileStream(caminho, FileMode.Create))
                    await fotoVistoria.CopyToAsync(stream);

                checklist.CaminhoFoto = "/fotosVistorias/" + nome;
            }

            _context.ChecklistsVistorias.Add(checklist);
            await _context.SaveChangesAsync();

            return RedirectToAction("Lista");
        }

        // ASSINATURA MANUAL
        [HttpPost]
        public IActionResult AssinarLaudo(int id, string nomeAssinatura)
        {
            var vistoria = _context.ChecklistsVistorias.FirstOrDefault(v => v.Id == id);
            if (vistoria == null) return NotFound();

            vistoria.AssinadoPor = nomeAssinatura;
            vistoria.DataAssinatura = DateTime.Now;
            vistoria.LaudoAssinado = true;

            _context.SaveChanges();
            return RedirectToAction("Detalhes", new { id });
        }

        // ALTERAR STATUS
        [HttpPost]
        public IActionResult AtualizarStatus(int id, string novoStatus)
        {
            var vistoria = _context.ChecklistsVistorias.FirstOrDefault(v => v.Id == id);
            if (vistoria == null)
                return Json(new { sucesso = false });

            vistoria.Status = novoStatus;
            _context.SaveChanges();

            return Json(new { sucesso = true });
        }

        // PDF COMPLETO
        public IActionResult BaixarPDF(int id)
        {
            var vistoria = _context.ChecklistsVistorias.FirstOrDefault(v => v.Id == id);
            if (vistoria == null) return NotFound();

            using (MemoryStream ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // TÍTULO
                Paragraph titulo = new Paragraph("LAUDO TÉCNICO DE VISTORIA",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22));
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                doc.Add(new Paragraph(" "));
                doc.Add(new LineSeparator());
                doc.Add(new Paragraph(" "));

                // DADOS
                PdfPTable dados = new PdfPTable(2);
                dados.WidthPercentage = 100;
                dados.AddCell("Imóvel:");
                dados.AddCell(vistoria.Imovel);
                dados.AddCell("Responsável:");
                dados.AddCell(vistoria.Responsavel);
                dados.AddCell("Status:");
                dados.AddCell(vistoria.Status);

                doc.Add(dados);
                doc.Add(new Paragraph(" "));
                doc.Add(new LineSeparator());
                doc.Add(new Paragraph(" "));

                // FOTO
                doc.Add(new Paragraph("FOTO DA VISTORIA",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));
                doc.Add(new Paragraph(" "));

                if (!string.IsNullOrEmpty(vistoria.CaminhoFoto))
                {
                    string caminhoOriginal = Path.Combine(_env.WebRootPath, vistoria.CaminhoFoto.TrimStart('/'));

                    if (System.IO.File.Exists(caminhoOriginal))
                    {
                        try
                        {
                            string ext = Path.GetExtension(caminhoOriginal).ToLower();
                            string[] diretos = { ".jpg", ".jpeg", ".png" };

                            string caminhoParaPdf = caminhoOriginal;

                            if (!diretos.Contains(ext))
                            {
                                string novoNome = Guid.NewGuid() + ".jpg";
                                string novoCaminho = Path.Combine(_env.WebRootPath, "fotosVistorias", novoNome);

                                using (var img = new MagickImage(caminhoOriginal))
                                {
                                    img.Format = MagickFormat.Jpeg;
                                    img.Write(novoCaminho);
                                }

                                caminhoParaPdf = novoCaminho;
                            }

                            var foto = iTextSharp.text.Image.GetInstance(caminhoParaPdf);
                            foto.ScaleToFit(450f, 350f);
                            foto.Alignment = Element.ALIGN_CENTER;
                            doc.Add(foto);
                        }
                        catch { }
                    }
                }

                // ASSINATURA
                doc.Add(new Paragraph(" "));
                doc.Add(new LineSeparator());
                doc.Add(new Paragraph(" "));

                doc.Add(new Paragraph("ASSINATURA DIGITAL",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));

                if (vistoria.LaudoAssinado)
                {
                    doc.Add(new Paragraph($"Assinado por: {vistoria.AssinadoPor}"));
                    doc.Add(new Paragraph($"Data da assinatura: {vistoria.DataAssinatura:dd/MM/yyyy HH:mm}"));
                }
                else
                {
                    doc.Add(new Paragraph("❌ Laudo não assinado."));
                }

                doc.Close();

                return File(ms.ToArray(), "application/pdf",
                    $"Laudo_{vistoria.Id}.pdf");
            }
        }
    }
}
