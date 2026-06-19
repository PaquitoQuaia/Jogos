
using System;

class Blackjack
{
    // Função para gerar uma carta aleatória
    static int DarCarta(Random random)
    {
        return random.Next(1, 12);
    }

    // Função para calcular o total da mão
    static int CalcularTotal(int[] mao, int quantidade)
    {
        int total = 0;

        for (int i = 0; i < quantidade; i++)
        {
            total += mao[i];
        }

        return total;
    }

    // Função para mostrar as cartas da mão
    static void MostrarMao(int[] mao, int quantidade)
    {
        for (int i = 0; i < quantidade; i++)
        {
            Console.Write($"[{mao[i]}] ");
        }

        Console.WriteLine();
    }

    static void Main()
    {
        int vitoriasJogador = 0;
        int vitoriasDealer = 0;
        Random random = new Random();
        bool jogando = true;

        Console.WriteLine("♠ ♥ ♣ ♦  BEM-VINDO AO BLACKJACK!  ♦ ♣ ♥ ♠");

        while (jogando)
        {
            Console.WriteLine("─────────────────────────");
            Console.WriteLine($"│  Você: {vitoriasJogador} x Dealer: {vitoriasDealer}  │");
            Console.WriteLine("─────────────────────────");

            Console.WriteLine("\nO que deseja fazer?");
            Console.WriteLine("1 - Nova Rodada");
            Console.WriteLine("2 - Sair");
            Console.Write("Escolha: ");

            string menuPrincipal = Console.ReadLine();

            switch (menuPrincipal)
            {
                case "1":

                    int[] maoJogador = new int[11];
                    int[] maoDealer = new int[11];
                    int qtdJogador = 0;
                    int qtdDealer = 0;

                    // Distribuição inicial das cartas
                    maoJogador[qtdJogador++] = DarCarta(random);
                    maoJogador[qtdJogador++] = DarCarta(random);
                    maoDealer[qtdDealer++] = DarCarta(random);
                    maoDealer[qtdDealer++] = DarCarta(random);

                    bool vezDoJogador = true;

                    while (vezDoJogador)
                    {
                        int totalJogador = CalcularTotal(maoJogador, qtdJogador);

                        Console.Write("\nDealer: ");

                        for (int i = 0; i < qtdDealer; i++)
                        {
                            if (i == qtdDealer - 1)
                                Console.Write("[?] ");
                            else
                                Console.Write($"[{maoDealer[i]}] ");
                        }

                        Console.Write("\nVocê:   ");
                        MostrarMao(maoJogador, qtdJogador);

                        Console.WriteLine($"Total: {totalJogador}");

                        if (totalJogador > 21)
                        {
                            Console.WriteLine("Você estourou! Dealer vence!");
                            vitoriasDealer++;
                            vezDoJogador = false;
                            break;
                        }

                        if (totalJogador == 21)
                        {
                            Console.WriteLine("BLACKJACK! Você venceu!");
                            vitoriasJogador++;
                            vezDoJogador = false;
                            break;
                        }

                        Console.WriteLine("\n1 - Pedir carta");
                        Console.WriteLine("2 - Parar");
                        Console.WriteLine("3 - Sair do jogo");
                        Console.Write("Escolha: ");

                        string acaoJogador = Console.ReadLine();

                        switch (acaoJogador)
                        {
                            case "1":
                                maoJogador[qtdJogador++] = DarCarta(random);
                                break;

                            case "2":
                                vezDoJogador = false;
                                break;

                            case "3":
                                Console.WriteLine("Saindo...");
                                jogando = false;
                                vezDoJogador = false;
                                break;

                            default:
                                Console.WriteLine("Opção inválida!");
                                break;
                        }
                    }

                    if (jogando)
                    {
                        int totalJogadorFinal = CalcularTotal(maoJogador, qtdJogador);

                        if (totalJogadorFinal <= 21)
                        {
                            Console.WriteLine("\n--- VEZ DO DEALER ---");

                            Console.Write("Dealer revela: ");
                            MostrarMao(maoDealer, qtdDealer);

                            while (CalcularTotal(maoDealer, qtdDealer) < 17)
                            {
                                int novaCarta = DarCarta(random);
                                maoDealer[qtdDealer++] = novaCarta;
                                Console.WriteLine($"Dealer pede carta: [{novaCarta}]");
                            }

                            int totalDealerFinal = CalcularTotal(maoDealer, qtdDealer);

                            Console.Write("Mão final do Dealer: ");
                            MostrarMao(maoDealer, qtdDealer);

                            Console.WriteLine("\n--- RESULTADO ---");
                            Console.WriteLine($"Seu total:    {totalJogadorFinal}");
                            Console.WriteLine($"Total Dealer: {totalDealerFinal}");
                            Console.WriteLine();

                            if (totalDealerFinal > 21)
                            {
                                Console.WriteLine("Dealer estourou! Você venceu!");
                                vitoriasJogador++;
                            }
                            else if (totalJogadorFinal > totalDealerFinal)
                            {
                                Console.WriteLine("Você venceu!");
                                vitoriasJogador++;
                            }
                            else if (totalDealerFinal > totalJogadorFinal)
                            {
                                Console.WriteLine("Dealer venceu!");
                                vitoriasDealer++;
                            }
                            else
                            {
                                Console.WriteLine("Empate!");
                            }
                        }
                    }

                    break;

                case "2":
                    Console.WriteLine("Obrigado por jogar! Até logo!");
                    jogando = false;
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
        }

        Console.WriteLine("\n─── PLACAR FINAL ───");
        Console.WriteLine($"Você: {vitoriasJogador} x Dealer: {vitoriasDealer}");
    }
}