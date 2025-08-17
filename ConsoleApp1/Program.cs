using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1 {
    internal class Program {
        static void Main(string[] args) {
            bool continuar = true;

            while (continuar) {
                var threadIdsSeq = new HashSet<int>();
                var threadIdsPar = new HashSet<int>();

                Console.Write("Digite o número de iterações para o teste (padrão: 30): ");
                string entrada = Console.ReadLine();
                int iteracoes = string.IsNullOrWhiteSpace(entrada) ? 30 : int.Parse(entrada);

                Console.Write("Digite o tempo simulado por iteração em segundos (padrão: 1): ");
                string tempoEntrada = Console.ReadLine();
                int tempoSegundos = string.IsNullOrWhiteSpace(tempoEntrada) ? 1 : int.Parse(tempoEntrada);
                int tempoMillis = tempoSegundos * 1000;

                Console.WriteLine("\n=== INICIANDO TESTES ===\n");

                Console.WriteLine("\n=== SEQUENCIAL ===\n");
                var swSeq = Stopwatch.StartNew();
                for (int i = 0; i < iteracoes; i++) {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    threadIdsSeq.Add(threadId);

                    Console.Write($"{i}");
                    if (i < iteracoes - 1) {
                        Console.Write(", ");
                    }
                    Thread.Sleep(tempoMillis);
                }
                swSeq.Stop();

                Console.WriteLine("\n\n=== PARALELO ===\n");
                var swPar = Stopwatch.StartNew();
                Parallel.For(0, iteracoes, i => {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    lock (threadIdsPar) {
                        threadIdsPar.Add(threadId);
                    }

                    Console.Write($"{i}");
                    if (i < iteracoes - 1) {
                        Console.Write(", ");
                    }
                    Thread.Sleep(tempoMillis);
                });
                swPar.Stop();

                Console.WriteLine();
                Console.WriteLine("\n=== RESULTADOS ===");

                string FormatTempo(TimeSpan ts) {
                    if (ts.TotalHours >= 1)
                        return $"{(int)ts.TotalHours}h {ts.Minutes}m {ts.Seconds}s";
                    if (ts.TotalMinutes >= 1)
                        return $"{ts.Minutes}m {ts.Seconds}s";
                    return $"{ts.Seconds}s";
                }

                Console.WriteLine($"[Sequencial] Tempo total: {swSeq.ElapsedMilliseconds} ms ({FormatTempo(swSeq.Elapsed)})");
                Console.WriteLine($"[Sequencial] Total de threads distintas: {threadIdsSeq.Count}");
                Console.WriteLine();
                Console.WriteLine($"[Paralelo] Tempo total: {swPar.ElapsedMilliseconds} ms ({FormatTempo(swPar.Elapsed)})");
                Console.WriteLine($"[Paralelo] Total de threads distintas: {threadIdsPar.Count}");
                Console.WriteLine();

                double diferenca = swSeq.Elapsed.TotalSeconds - swPar.Elapsed.TotalSeconds;
                if (diferenca > 0) {
                    Console.WriteLine($"\nO paralelo foi {diferenca:F2} segundos MAIS rápido que o sequencial.");
                } else if (diferenca < 0) {
                    Console.WriteLine($"\nO paralelo foi {Math.Abs(diferenca):F2} segundos MAIS lento que o sequencial.");
                } else {
                    Console.WriteLine("\nAmbos executaram no mesmo tempo.");
                }

                Console.WriteLine("\nDeseja fazer um novo teste? (S/N): ");
                string resposta = Console.ReadLine().Trim().ToUpper();
                if (resposta == "N") {
                    continuar = false;
                } else {
                    Console.Clear();
                }
            }
        }
    }
}
