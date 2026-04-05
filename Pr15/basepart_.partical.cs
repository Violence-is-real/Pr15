using System.Collections.Generic;
using System.Linq;

namespace Pr15
{
    public partial class basepart_
    {
        private string GetSocketName(int? id) => Core.Context.socket_.Find(id)?.name ?? "—";
        private string GetMemoryType(int? id) => Core.Context.memorytype_.Find(id)?.name ?? "—";
        private string GetFormFactor(int? id) => Core.Context.formfactor_.Find(id)?.name ?? "—";
        private string GetCertificate(int? id) => Core.Context.certificate_.Find(id)?.name ?? "—";
        public string Details
        {
            get
            {
                var info = new List<string>();
                switch (parttypeid)
                {
                    case 1:
                        var cpu = Core.Context.cpu_.Find(id);
                        if (cpu != null)
                        {
                            info.Add($"Сокет: {GetSocketName(cpu.socketid)}");
                            info.Add($"Ядер: {cpu.numberofcores}");
                            info.Add($"Частота: {cpu.basecorefrequency}-{cpu.maxcorefrequency} ГГц");
                            info.Add($"Кэш L3: {cpu.cachel3} МБ");
                            info.Add($"TDP: {cpu.thermalpower} Вт");
                        }
                        break;

                }
                return info.Count == 0 ? "—" : string.Join(" | ", info);
            }
        }
    }
    }
