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

        public IActionResult Lista(string status = "Todas")
        {
            var query = _context.ChecklistsVistorias.AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "Todas")
                query = query.Where(v => v.Status == status);

            var vistorias = query.OrderByDescending(v => v.Data).ToList();

            ViewBag.Total = _context.ChecklistsVistorias.Count();
            ViewBag.Concluidas = _context.ChecklistsVistorias.Count(v => v.Status == "Concluída");
            ViewBag.Agendadas = _context.ChecklistsVistorias.Count(v => v.Status == "Agendada");
            ViewBag.FiltroAtual = status;

            return View(vistorias);
        }

        public IActionResult Detalhes(int id)
        {
            var vistoria = _context.ChecklistsVistorias.FirstOrDefault(v => v.Id == id);
            if (vistoria == null)
                return NotFound();

            return View(vistoria);
        }

        [HttpGet]
        public IActionResult Nova() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Nova(ChecklistVistoria checklist)
        {
            return View("ChecklistCompleto", checklist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            var vistoria = _context.ChecklistsVistorias.Find(id);
            if (vistoria == null) return NotFound();

            if (!string.IsNullOrEmpty(vistoria.CaminhoFoto))
            {
                string caminhoFisico = Path.Combine(_env.WebRootPath, vistoria.CaminhoFoto.TrimStart('/'));
                if (System.IO.File.Exists(caminhoFisico))
                    System.IO.File.Delete(caminhoFisico);
            }

            _context.ChecklistsVistorias.Remove(vistoria);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Vistoria excluída com sucesso!";
            return RedirectToAction("Lista");
        }

        [HttpGet]
        public IActionResult ChecklistCompleto(ChecklistVistoria checklist)
        {
            if (checklist == null)
                checklist = new ChecklistVistoria();

            return View(checklist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarChecklistCompleto(ChecklistVistoria checklist, IFormFile fotoVistoria)
        {
            if (string.IsNullOrEmpty(checklist.Imovel)) checklist.Imovel = "Não informado";
            if (string.IsNullOrEmpty(checklist.Responsavel)) checklist.Responsavel = "Não informado";
            if (string.IsNullOrEmpty(checklist.Status)) checklist.Status = "Entrada";
            if (checklist.Data == default) checklist.Data = DateTime.Now;
            if (string.IsNullOrEmpty(checklist.Horario)) checklist.Horario = DateTime.Now.ToString("HH:mm");
            if (string.IsNullOrEmpty(checklist.Observacoes)) checklist.Observacoes = "";

            if (fotoVistoria != null)
            {
                string pasta = Path.Combine(_env.WebRootPath, "fotosVistorias");
                Directory.CreateDirectory(pasta);

                string nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(fotoVistoria.FileName);
                string caminhoFisico = Path.Combine(pasta, nomeArquivo);

                using (var stream = new FileStream(caminhoFisico, FileMode.Create))
                    await fotoVistoria.CopyToAsync(stream);

                checklist.CaminhoFoto = "/fotosVistorias/" + nomeArquivo;
            }

            _context.Add(checklist);
            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Checklist salvo com sucesso!";
            return RedirectToAction("Lista");
        }

        // ✅ GERAÇÃO DE PDF FUNCIONAL E CORRIGIDA
        [HttpGet]
        public IActionResult BaixarPDF(int id)
        {
            var vistoria = _context.ChecklistsVistorias.FirstOrDefault(v => v.Id == id);
            if (vistoria == null)
                return NotFound();

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // === Cabeçalho ===
                var fonteTitulo = FontFactory.GetFont("Helvetica", 18, Font.BOLD, BaseColor.Black);
                var titulo = new Paragraph($"Checklist de Vistoria - {vistoria.Imovel}", fonteTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph($"Responsável: {vistoria.Responsavel}"));
                doc.Add(new Paragraph($"Data: {vistoria.Data:dd/MM/yyyy}  •  Horário: {vistoria.Horario}"));
                doc.Add(new Paragraph($"Status: {vistoria.Status}"));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph("Observações: " + (string.IsNullOrEmpty(vistoria.Observacoes) ? "Nenhuma" : vistoria.Observacoes)));
                doc.Add(new Paragraph(" "));

                // === Foto ===
                if (!string.IsNullOrEmpty(vistoria.CaminhoFoto))
                {
                    try
                    {
                        string caminhoFisico = Path.Combine(_env.WebRootPath, vistoria.CaminhoFoto.TrimStart('/'));
                        if (System.IO.File.Exists(caminhoFisico))
                        {
                            var img = iTextSharp.text.Image.GetInstance(caminhoFisico);
                            img.ScaleToFit(400f, 300f);
                            img.Alignment = Element.ALIGN_CENTER;
                            doc.Add(img);
                            doc.Add(new Paragraph(" "));
                        }
                    }
                    catch { }
                }

                // === Itens verificados ===
                var fonteSubtitulo = FontFactory.GetFont("Helvetica", 14, Font.BOLD, BaseColor.Black);
                doc.Add(new Paragraph("Itens Verificados:", fonteSubtitulo));
                doc.Add(new Paragraph(" "));

                var props = typeof(ChecklistVistoria).GetProperties()
                    .Where(p => p.PropertyType == typeof(bool))
                    .ToList();

                foreach (var p in props)
                {
                    bool valor = (bool)(p.GetValue(vistoria) ?? false);
                    string nome = p.Name.Replace("_", " ");
                    var linha = new Paragraph($"{(valor ? "✔" : "❌")} {nome}");
                    doc.Add(linha);
                }

                doc.Close();
                var bytes = ms.ToArray();

                return File(bytes, "application/pdf", $"Vistoria_{vistoria.Imovel}_{vistoria.Id}.pdf");
            }
        }
    }
}
