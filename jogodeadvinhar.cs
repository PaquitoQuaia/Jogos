using System;

class JogoDeAdvinhar
{
    static void Main()
    {
        bool rodando = true;

        while (rodando)
        {
            Console.WriteLine("Bem-vindo ao Jogo de Adivinhação!");
            Console.WriteLine();
            Console.WriteLine("1 - JOGAR");
            Console.WriteLine("2 - REGRAS");
            Console.WriteLine("3 - SAIR");

            string saida1 = Console.ReadLine();

            switch (saida1)
            {
                case "1":
                    Random random = new Random();
                    int numeroSecreto = random.Next(1, 101);
                    int tentativas = 0;
                    bool acertou = false;

                    while (!acertou)
                    {
                        Console.WriteLine("Digite seu palpite: ");
                        int palpite = int.Parse(Console.ReadLine());
                        tentativas++;

                        if (palpite < numeroSecreto)
                            Console.WriteLine("O palpite é menor que o número! Tente novamente!");
                        else if (palpite > numeroSecreto)
                            Console.WriteLine("O palpite é maior que o número! Tente novamente!");
                        else
                        {
                            acertou = true;
                            Console.WriteLine($"Parabéns! Você acertou em {tentativas} tentativas");
                        }
                    }
                    break;

                case "2":
                    Console.WriteLine("1 - Palpite um número de 1-100");
                    Console.WriteLine("2 - Palpite a cada rodada");
                    Console.WriteLine("3 - Acerte no minimo de tentativas possiveis!");
                    Console.WriteLine("Boa Sorte!!!!");
                    break; 

                case "3":
                    Console.WriteLine("Saindo...");
                    rodando = false; 
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
        }
    }
}