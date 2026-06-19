using System;
using System.Collections.Generic;
using System.Threading;

class JogoDaCobrinha
{
    // Dimensões do tabuleiro
    private const int LARGURA = 40;
    private const int ALTURA = 20;

    // Lista que armazena as posições da cobra (x, y)
    private List<(int, int)> cobra;

    // Posição da fruta
    private (int, int) fruta;

    // Direção atual da cobra (dx, dy)
    private int direcaoX = 1;
    private int direcaoY = 0;

    // Próxima direção (evita a cobra virar 180 graus instantaneamente)
    private int proximaDirecaoX = 1;
    private int proximaDirecaoY = 0;

    // Controle do jogo
    private bool jogoAtivo = true;
    private int pontuacao = 0;
    private int velocidade = 100; // Milissegundos entre cada movimento
    private Random random = new Random();

    public JogoDaCobrinha()
    {
        // Inicializa a cobra no centro do tabuleiro com 3 segmentos
        cobra = new List<(int, int)>();
        cobra.Add((LARGURA / 2, ALTURA / 2));
        cobra.Add((LARGURA / 2 - 1, ALTURA / 2));
        cobra.Add((LARGURA / 2 - 2, ALTURA / 2));

        // Gera a primeira fruta
        GerarFruta();
    }

    // ========== FUNÇÃO 1: DESENHAR O TABULEIRO ==========
    // Responsável por renderizar a cobra, fruta e bordas do jogo
    private void DesenharTabuleiro()
    {
        Console.Clear();

        // Desenha a borda superior
        Console.SetCursorPosition(0, 0);
        for (int x = 0; x < LARGURA + 2; x++)
            Console.Write("█");

        // Desenha as laterais e o interior do tabuleiro
        for (int y = 1; y < ALTURA + 1; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write("█"); // Borda esquerda

            Console.SetCursorPosition(LARGURA + 1, y);
            Console.Write("█"); // Borda direita
        }

        // Desenha a borda inferior
        Console.SetCursorPosition(0, ALTURA + 1);
        for (int x = 0; x < LARGURA + 2; x++)
            Console.Write("█");

        // ===== FOR: Desenha a cobra =====
        for (int i = 0; i < cobra.Count; i++)
        {
            int x = cobra[i].Item1 + 1; // +1 para não sobrescrever a borda
            int y = cobra[i].Item2 + 1;

            Console.SetCursorPosition(x, y);

            // A cabeça é diferente do corpo
            if (i == 0)
                Console.Write("◆"); // Cabeça
            else
                Console.Write("●"); // Corpo
        }

        // Desenha a fruta
        Console.SetCursorPosition(fruta.Item1 + 1, fruta.Item2 + 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("★");
        Console.ResetColor();

        // Desenha o placar
        Console.SetCursorPosition(0, ALTURA + 3);
        Console.WriteLine($"Pontuação: {pontuacao} | Velocidade: {101 - velocidade} | SETAS para mover | ESC para sair");
    }

    // ========== FUNÇÃO 2: GERAR FRUTA EM POSIÇÃO ALEATÓRIA ==========
    private void GerarFruta()
    {
        bool posicaoValida = false;

        while (!posicaoValida)
        {
            int x = random.Next(0, LARGURA);
            int y = random.Next(0, ALTURA);

            // Verifica se a fruta não aparece dentro da cobra
            posicaoValida = true;
            foreach (var segmento in cobra)
            {
                if (segmento.Item1 == x && segmento.Item2 == y)
                {
                    posicaoValida = false;
                    break;
                }
            }

            if (posicaoValida)
                fruta = (x, y);
        }
    }

    // ========== FUNÇÃO 3: MOVER A COBRA ==========
    // Atualiza a direção e move a cobra de acordo
    private void MoverCobra()
    {
        // Atualiza a direção com o input do jogador
        direcaoX = proximaDirecaoX;
        direcaoY = proximaDirecaoY;

        // Calcula a nova posição da cabeça
        int novaX = cobra[0].Item1 + direcaoX;
        int novaY = cobra[0].Item2 + direcaoY;

        // Insere novo segmento na frente
        cobra.Insert(0, (novaX, novaY));

        // ===== IF: Verifica se comeu a fruta =====
        if (novaX == fruta.Item1 && novaY == fruta.Item2)
        {
            // A cobra cresce automaticamente (não remove a cauda)
            pontuacao += 10;

            // Aumenta a velocidade (reduz o delay)
            if (velocidade > 30)
                velocidade -= 2;

            GerarFruta();
        }
        else
        {
            // Remove a cauda se não comeu a fruta
            cobra.RemoveAt(cobra.Count - 1);
        }
    }

    // ========== FUNÇÃO 4: VERIFICAR COLISÕES ==========
    // ===== IF/ELSE: Detecta colisão com parede ou com o próprio corpo =====
    private bool VerificarColisao()
    {
        int cabecaX = cobra[0].Item1;
        int cabecaY = cobra[0].Item2;

        // IF: Colisão com as paredes
        if (cabecaX < 0 || cabecaX >= LARGURA || cabecaY < 0 || cabecaY >= ALTURA)
            return true;

        // IF: Colisão com o corpo da cobra (começa do índice 1 para não contar a cabeça)
        for (int i = 1; i < cobra.Count; i++)
        {
            if (cobra[i].Item1 == cabecaX && cobra[i].Item2 == cabecaY)
                return true;
        }

        return false;
    }

    // ========== FUNÇÃO 5: PROCESSAR INPUT DO TECLADO ==========
    // ===== SWITCH/CASE: Processa a seta pressionada =====
    private void ProcessarInput()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo tecla = Console.ReadKey(true);

            switch (tecla.Key)
            {
                case ConsoleKey.UpArrow:
                    // Seta para cima - só muda se não estava indo para baixo
                    if (direcaoY != 1)
                    {
                        proximaDirecaoX = 0;
                        proximaDirecaoY = -1;
                    }
                    break;

                case ConsoleKey.DownArrow:
                    // Seta para baixo - só muda se não estava indo para cima
                    if (direcaoY != -1)
                    {
                        proximaDirecaoX = 0;
                        proximaDirecaoY = 1;
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    // Seta para esquerda - só muda se não estava indo para direita
                    if (direcaoX != 1)
                    {
                        proximaDirecaoX = -1;
                        proximaDirecaoY = 0;
                    }
                    break;

                case ConsoleKey.RightArrow:
                    // Seta para direita - só muda se não estava indo para esquerda
                    if (direcaoX != -1)
                    {
                        proximaDirecaoX = 1;
                        proximaDirecaoY = 0;
                    }
                    break;

                case ConsoleKey.Escape:
                    // ESC para sair do jogo
                    jogoAtivo = false;
                    break;
            }
        }
    }

    // ========== LOOP PRINCIPAL DO JOGO ==========
    // ===== WHILE: Executa enquanto o jogo está ativo =====
    public void Iniciar()
    {
        Console.CursorVisible = false;

        while (jogoAtivo)
        {
            DesenharTabuleiro();
            ProcessarInput();
            MoverCobra();

            // IF: Verifica se houve colisão
            if (VerificarColisao())
            {
                jogoAtivo = false;
            }

            // Thread.Sleep controla a velocidade do jogo
            Thread.Sleep(velocidade);
        }

        // Exibe mensagem final
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║        FIM DE JOGO - GAME OVER         ║");
        Console.WriteLine("║────────────────────────────────────────║");
        Console.WriteLine($"║ Pontuação Final: {pontuacao,-27} ║");
        Console.WriteLine($"║ Comprimento da Cobra: {cobra.Count,-19} ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.CursorVisible = true;
    }

    // ========== MAIN: PONTO DE ENTRADA ==========
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        JogoDaCobrinha jogo = new JogoDaCobrinha();
        jogo.Iniciar();
    }
}