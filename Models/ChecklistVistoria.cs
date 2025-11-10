using System;

namespace Vistoria_projeto.Models
{
    public class ChecklistVistoria
    {
        public int Id { get; set; }

        // Cabeçalho / dados gerais
        public DateTime Data { get; set; } = DateTime.Now;
        public string? Horario { get; set; }
        public string Responsavel { get; set; }
        public string Imovel { get; set; }
        public string Status { get; set; } // "Entrada", "Saída", "Agendada", "Concluída"

        // Paredes e Tetos
        public bool PinturaBomEstado { get; set; }
        public bool SemRachaduras { get; set; }
        public bool SemInfiltracoes { get; set; }
        public bool SemManchas { get; set; }

        // Pisos
        public bool PisoBomEstado { get; set; }
        public bool RodapesConservados { get; set; }
        public bool SemTrincas { get; set; }

        // Portas e Janelas
        public bool PortasFuncionando { get; set; }
        public bool FechadurasOk { get; set; }
        public bool JanelasVedando { get; set; }
        public bool VidrosIntactos { get; set; }

        // Elétrica
        public bool InterruptoresOk { get; set; }
        public bool TomadasFuncionando { get; set; }
        public bool IluminacaoOk { get; set; }
        public bool QuadroLuzOk { get; set; }

        // Hidráulica
        public bool TorneirasSemVazamento { get; set; }
        public bool ChuveiroOk { get; set; }
        public bool VasoSanitarioOk { get; set; }
        public bool RalosDesentupidos { get; set; }

        // Cozinha
        public bool PiaBomEstado { get; set; }
        public bool ArmariosCozinhaOk { get; set; }
        public bool FogaoIncluido { get; set; }
        public bool GeladeiraIncluida { get; set; }

        // Banheiro
        public bool BoxBomEstado { get; set; }
        public bool EspelhosOk { get; set; }
        public bool ArmariosBanheiroOk { get; set; }
        public bool AcabamentosOk { get; set; }

        // Observações gerais + Foto
        public string Observacoes { get; set; }
        public string CaminhoFoto { get; set; }
    }
}
