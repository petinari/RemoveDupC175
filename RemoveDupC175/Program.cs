using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoveDupC175
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Digite o caminho do arquivo sped contribuições: ");
            string? path = Console.ReadLine()?.Replace("\"", "");

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path).ToList();
                RemoveDuplicateC175Records(lines);
                UpdateLineCounts(lines);

                string newPath = Path.ChangeExtension(path, "_new.txt");

                File.WriteAllLines(newPath, lines);

                Console.WriteLine("Finalizado!");
            }
            else
            {
                Console.WriteLine("Arquivo não encontrado.");
            }
        }

        static void RemoveDuplicateC175Records(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("|C100|"))
                {
                    var totalDeC175 = new List<string>();

                    for (int j = i + 1; j < lines.Count; j++)
                    {
                        if (lines[j].StartsWith("|C175|"))
                        {
                            totalDeC175.Add(lines[j]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (totalDeC175.Count > 1)
                    {
                        var qtdOriginalC175 = totalDeC175.Count + 1;
                        var (totalVL_OPR, totalVL_BC_PIS, totalVL_PIS, totalVL_BC_COFINS, totalVL_COFINS) =
                            CalculateTotals(totalDeC175);

                        totalDeC175.RemoveRange(1, totalDeC175.Count - 1);

                        string[] line3 = totalDeC175[0].Split('|');

                        line3[3] = totalVL_OPR.ToString("F2");
                        line3[6] = totalVL_BC_PIS.ToString("F2");
                        line3[10] = totalVL_PIS.ToString("F2");
                        line3[12] = totalVL_BC_COFINS.ToString("F2");
                        line3[16] = totalVL_COFINS.ToString("F2");

                        totalDeC175[0] = string.Join("|", line3);

                        for (int l = 1; l < qtdOriginalC175; l++)
                        {
                            lines.RemoveAt(i + 1);
                        }

                        lines.Insert(i + 1, totalDeC175[0]);
                    }
                }
            }
        }

        static (double, double, double, double, double) CalculateTotals(List<string> totalDeC175)
        {
            double totalVL_OPR = 0.0;
            double totalVL_BC_PIS = 0.0;
            double totalVL_PIS = 0.0;
            double totalVL_BC_COFINS = 0.0;
            double totalVL_COFINS = 0.0;

            foreach (string line in totalDeC175)
            {
                string[] line2 = line.Split('|');
                totalVL_OPR += Convert.ToDouble(line2[3]);
                totalVL_BC_PIS += Convert.ToDouble(line2[6]);
                totalVL_PIS += Convert.ToDouble(line2[10]);
                totalVL_BC_COFINS += Convert.ToDouble(line2[12]);
                totalVL_COFINS += Convert.ToDouble(line2[16]);
            }

            return (totalVL_OPR, totalVL_BC_PIS, totalVL_PIS, totalVL_BC_COFINS, totalVL_COFINS);
        }

        static void UpdateLineCounts(List<string> lines)
        {
            int countC = lines.Count(s => s.StartsWith("|C"));
            int count = lines.Count;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("|C990|"))
                {
                    string[] line = lines[i].Split('|');
                    line[2] = countC.ToString();
                    lines[i] = string.Join("|", line);
                }
                else if (lines[i].StartsWith("|9999|"))
                {
                    string[] line = lines[i].Split('|');
                    line[2] = count.ToString();
                    lines[i] = string.Join("|", line);
                }
            }
        }
    }
}
