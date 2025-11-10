using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vistoria_projeto.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistVistoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecklistsVistorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Horario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsavel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Imovel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PinturaBomEstado = table.Column<bool>(type: "bit", nullable: false),
                    SemRachaduras = table.Column<bool>(type: "bit", nullable: false),
                    SemInfiltracoes = table.Column<bool>(type: "bit", nullable: false),
                    SemManchas = table.Column<bool>(type: "bit", nullable: false),
                    PisoBomEstado = table.Column<bool>(type: "bit", nullable: false),
                    RodapesConservados = table.Column<bool>(type: "bit", nullable: false),
                    SemTrincas = table.Column<bool>(type: "bit", nullable: false),
                    PortasFuncionando = table.Column<bool>(type: "bit", nullable: false),
                    FechadurasOk = table.Column<bool>(type: "bit", nullable: false),
                    JanelasVedando = table.Column<bool>(type: "bit", nullable: false),
                    VidrosIntactos = table.Column<bool>(type: "bit", nullable: false),
                    InterruptoresOk = table.Column<bool>(type: "bit", nullable: false),
                    TomadasFuncionando = table.Column<bool>(type: "bit", nullable: false),
                    IluminacaoOk = table.Column<bool>(type: "bit", nullable: false),
                    QuadroLuzOk = table.Column<bool>(type: "bit", nullable: false),
                    TorneirasSemVazamento = table.Column<bool>(type: "bit", nullable: false),
                    ChuveiroOk = table.Column<bool>(type: "bit", nullable: false),
                    VasoSanitarioOk = table.Column<bool>(type: "bit", nullable: false),
                    RalosDesentupidos = table.Column<bool>(type: "bit", nullable: false),
                    PiaBomEstado = table.Column<bool>(type: "bit", nullable: false),
                    ArmariosCozinhaOk = table.Column<bool>(type: "bit", nullable: false),
                    FogaoIncluido = table.Column<bool>(type: "bit", nullable: false),
                    GeladeiraIncluida = table.Column<bool>(type: "bit", nullable: false),
                    BoxBomEstado = table.Column<bool>(type: "bit", nullable: false),
                    EspelhosOk = table.Column<bool>(type: "bit", nullable: false),
                    ArmariosBanheiroOk = table.Column<bool>(type: "bit", nullable: false),
                    AcabamentosOk = table.Column<bool>(type: "bit", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaminhoFoto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistsVistorias", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistsVistorias");
        }
    }
}
