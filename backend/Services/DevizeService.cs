using Backend.Data;
using Backend.DTOs.Deviz;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Backend.Services;

public class DevizeService : IDevizeService
{
    private readonly AppDbContext _db;

    public DevizeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<DevizDto>> GetAllAsync()
    {
        var devize = await _db.Devize
            .Include(d => d.Linii)
            .Include(d => d.Proiect)
            .OrderByDescending(d => d.Data)
            .ToListAsync();

        return devize.Select(MapToDto).ToList();
    }

    public async Task<List<DevizDto>> GetByProiectIdAsync(int proiectId)
    {
        var devize = await _db.Devize
            .Where(d => d.ProiectId == proiectId)
            .Include(d => d.Linii)
            .Include(d => d.Proiect)
            .OrderByDescending(d => d.Data)
            .ToListAsync();

        return devize.Select(MapToDto).ToList();
    }

    public async Task<DevizDto?> GetByIdAsync(int id)
    {
        var deviz = await _db.Devize
            .Include(d => d.Linii)
            .Include(d => d.Proiect)
            .FirstOrDefaultAsync(d => d.Id == id);

        return deviz == null ? null : MapToDto(deviz);
    }

    public async Task<DevizDto> CreateAsync(int proiectId, CreateDevizRequest request)
    {
        var deviz = new Deviz
        {
            ProiectId = proiectId,
            Titlu = request.Titlu,
            NumarInregistrare = request.NumarInregistrare,
            Beneficiar = request.Beneficiar,
            Executant = request.Executant,
            CotaTVA = request.CotaTVA,
            Data = request.Data,
            Linii = request.Linii.Select(l => new DevizLinie
            {
                Numar = l.Numar,
                Descriere = l.Descriere,
                UM = l.UM,
                Cantitate = l.Cantitate,
                PretUnitar = l.PretUnitar
            }).ToList()
        };

        _db.Devize.Add(deviz);
        await _db.SaveChangesAsync();

        return MapToDto(deviz);
    }

    public async Task<DevizDto?> UpdateAsync(int id, UpdateDevizRequest request)
    {
        var deviz = await _db.Devize
            .Include(d => d.Linii)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deviz == null) return null;

        if (request.Titlu != null) deviz.Titlu = request.Titlu;
        if (request.NumarInregistrare != null) deviz.NumarInregistrare = request.NumarInregistrare;
        if (request.Beneficiar != null) deviz.Beneficiar = request.Beneficiar;
        if (request.Executant != null) deviz.Executant = request.Executant;
        if (request.CotaTVA.HasValue) deviz.CotaTVA = request.CotaTVA.Value;
        if (request.Data.HasValue) deviz.Data = request.Data.Value;

        if (request.Linii != null)
        {
            _db.DevizLinii.RemoveRange(deviz.Linii);
            deviz.Linii = request.Linii.Select(l => new DevizLinie
            {
                DevizId = deviz.Id,
                Numar = l.Numar,
                Descriere = l.Descriere,
                UM = l.UM,
                Cantitate = l.Cantitate,
                PretUnitar = l.PretUnitar
            }).ToList();
        }

        await _db.SaveChangesAsync();
        return MapToDto(deviz);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deviz = await _db.Devize.FindAsync(id);
        if (deviz == null) return false;

        _db.Devize.Remove(deviz);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<byte[]> GeneratePdfAsync(int id)
    {
        var deviz = await _db.Devize
            .Include(d => d.Linii.OrderBy(l => l.Numar))
            .Include(d => d.Proiect)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deviz == null) return Array.Empty<byte>();

        QuestPDF.Settings.License = LicenseType.Community;

        var cotaTva = deviz.CotaTVA;
        var totalGeneral = deviz.Linii.Sum(l => l.Cantitate * l.PretUnitar);
        var tva = totalGeneral * cotaTva / 100m;
        var totalCuTva = totalGeneral + tva;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginVertical(40);
                page.MarginHorizontal(50);

                page.Content().Column(col =>
                {
                    col.Spacing(5);

                    // Row 1: Beneficiar + Nr. Înregistrare (colț dreapta sus)
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Beneficiar: ").Bold().FontSize(10);
                            text.Span(deviz.Beneficiar ?? "-").FontSize(10);
                        });
                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.Span("Nr. înreg.: ").FontSize(10);
                            text.Span(deviz.NumarInregistrare ?? "-").Bold().FontSize(10);
                        });
                    });

                    // Row 2: Executant + Data
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Executant: ").Bold().FontSize(10);
                            text.Span(deviz.Executant ?? "-").FontSize(10);
                        });
                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.Span("Data: ").FontSize(10);
                            text.Span(deviz.Data.ToString("dd.MM.yyyy")).Bold().FontSize(10);
                        });
                    });

                    // Row 3: Proiect
                    if (deviz.Proiect != null)
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("Proiect: ").Bold().FontSize(10);
                            text.Span(deviz.Proiect.Nume).FontSize(10);
                        });
                    }

                    // Separator
                    col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                    // Title
                    col.Item().AlignCenter().Text(deviz.Titlu).FontSize(14).Bold();

                    col.Item().PaddingTop(10);

                    // Main table
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(35);
                            columns.RelativeColumn();
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(85);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten2)
                                .Padding(5).AlignCenter().Text("Nr.").Bold().FontSize(9);
                            header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten2)
                                .Padding(5).Text("Capitol de lucrări").Bold().FontSize(9);
                            header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten2)
                                .Padding(5).AlignCenter().Text("U.M.").Bold().FontSize(9);
                            header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten2)
                                .Padding(5).AlignCenter().Text("Cantitate").Bold().FontSize(9);
                            header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten2)
                                .Padding(5).AlignCenter().Text(text =>
                                {
                                    text.Line("Preț unitar").Bold().FontSize(9);
                                    text.Line("(fără TVA)").FontSize(8);
                                });
                            header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten2)
                                .Padding(5).AlignCenter().Text(text =>
                                {
                                    text.Line("TOTAL").Bold().FontSize(9);
                                    text.Line("(fără TVA)").FontSize(8);
                                });
                        });

                        foreach (var linie in deviz.Linii)
                        {
                            var totalLinie = linie.Cantitate * linie.PretUnitar;

                            table.Cell().Border(0.5f).BorderColor(Colors.Grey.Medium)
                                .Padding(4).AlignCenter().Text(linie.Numar.ToString()).FontSize(9);
                            table.Cell().Border(0.5f).BorderColor(Colors.Grey.Medium)
                                .Padding(4).Text(linie.Descriere).FontSize(9);
                            table.Cell().Border(0.5f).BorderColor(Colors.Grey.Medium)
                                .Padding(4).AlignCenter().Text(linie.UM).FontSize(9);
                            table.Cell().Border(0.5f).BorderColor(Colors.Grey.Medium)
                                .Padding(4).AlignRight().Text(linie.Cantitate.ToString("N4")).FontSize(9);
                            table.Cell().Border(0.5f).BorderColor(Colors.Grey.Medium)
                                .Padding(4).AlignRight().Text(linie.PretUnitar.ToString("N2")).FontSize(9);
                            table.Cell().Border(0.5f).BorderColor(Colors.Grey.Medium)
                                .Padding(4).AlignRight().Text(totalLinie.ToString("N2")).FontSize(9);
                        }

                        // Total fără TVA
                        table.Cell().ColumnSpan(5).Border(1).BorderColor(Colors.Black)
                            .Background(Colors.Grey.Lighten2).Padding(5)
                            .AlignRight().Text("TOTAL GENERAL (fără TVA):").Bold().FontSize(9);
                        table.Cell().Border(1).BorderColor(Colors.Black)
                            .Background(Colors.Grey.Lighten2).Padding(5)
                            .AlignRight().Text(totalGeneral.ToString("N2") + " lei").Bold().FontSize(9);

                        // TVA cu cota variabilă
                        table.Cell().ColumnSpan(5).Border(0.5f).BorderColor(Colors.Grey.Medium)
                            .Padding(5).AlignRight().Text($"TVA {cotaTva:G29}%:").FontSize(9);
                        table.Cell().Border(0.5f).BorderColor(Colors.Grey.Medium)
                            .Padding(5).AlignRight().Text(tva.ToString("N2") + " lei").FontSize(9);

                        // Total cu TVA
                        table.Cell().ColumnSpan(5).Border(1).BorderColor(Colors.Black)
                            .Background(Colors.Grey.Lighten1).Padding(5)
                            .AlignRight().Text("TOTAL CU TVA:").Bold().FontSize(10);
                        table.Cell().Border(1).BorderColor(Colors.Black)
                            .Background(Colors.Grey.Lighten1).Padding(5)
                            .AlignRight().Text(totalCuTva.ToString("N2") + " lei").Bold().FontSize(10);
                    });

                    // Semnături
                    col.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Beneficiar,").Bold().FontSize(10);
                            c.Item().PaddingTop(30).Text("________________________").FontSize(10);
                        });
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().AlignRight().Text("Executant,").Bold().FontSize(10);
                            c.Item().PaddingTop(30).AlignRight().Text("________________________").FontSize(10);
                        });
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8));
                    text.Span("Pagina ");
                    text.CurrentPageNumber();
                    text.Span(" din ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private static DevizDto MapToDto(Deviz d) => new()
    {
        Id = d.Id,
        ProiectId = d.ProiectId,
        NumeProiect = d.Proiect?.Nume ?? string.Empty,
        Titlu = d.Titlu,
        NumarInregistrare = d.NumarInregistrare,
        Beneficiar = d.Beneficiar,
        Executant = d.Executant,
        CotaTVA = d.CotaTVA,
        Data = d.Data,
        Linii = d.Linii.OrderBy(l => l.Numar).Select(l => new DevizLinieDto
        {
            Id = l.Id,
            Numar = l.Numar,
            Descriere = l.Descriere,
            UM = l.UM,
            Cantitate = l.Cantitate,
            PretUnitar = l.PretUnitar
        }).ToList()
    };
}
