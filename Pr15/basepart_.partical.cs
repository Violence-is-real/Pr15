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
                    case 2:
                        var gpu = Core.Context.gpu_.Find(id);
                        if (gpu != null)
                        {
                            info.Add($"Видеопамять: {gpu.videomemory} ГБ");
                            info.Add($"Частота чипа: {gpu.chipfrequency} МГц");
                            info.Add($"Шина памяти: {gpu.memorybus} бит");
                            info.Add($"Рек. БП: {gpu.recommendpower} Вт");
                        }
                        break;
                    case 3:
                        var ram = Core.Context.ram_.Find(id);
                        if (ram != null)
                        {
                            info.Add($"Тип: {GetMemoryType(ram.memorytypeid)}");
                            info.Add($"Объём: {ram.capacity} ГБ");
                            info.Add($"Планок: {ram.count}");
                            info.Add($"Частота: {ram.ghz} МГц");
                            info.Add($"Тайминги: {ram.timings}");
                        }
                        break;
                    case 4:
                        var mb = Core.Context.motherboard_.Find(id);
                        if (mb != null)
                        {
                            info.Add($"Сокет: {GetSocketName(mb.socketid)}");
                            info.Add($"Форм-фактор: {GetFormFactor(mb.formfactorid)}");
                            info.Add($"Слотов памяти: {mb.memoryslots}");
                            info.Add($"PCIe слотов: {mb.pcislots}");
                            info.Add($"SATA портов: {mb.sataports}");
                        }
                        break;
                    case 6:
                        var psu = Core.Context.powersupply_.Find(id);
                        if (psu != null)
                        {
                            info.Add($"Мощность: {psu.power} Вт");
                            info.Add($"Сертификация: {GetCertificate(psu.certificationid)}");
                        }
                        break;
                }
                return info.Count == 0 ? "—" : string.Join(" | ", info);
            }
        }
    }
    }
