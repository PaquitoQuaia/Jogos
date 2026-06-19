/*
 * ============================================================
 *   JOGO DA COBRINHA — Snake Game
 *   UC1 — Lógica de Programação e Algoritmos
 *   Desafio Master
 * ============================================================
 *
 *  Como compilar e rodar:
 *    dotnet-script SnakeGame.cs
 *  OU criar um projeto:
 *    dotnet new console -n Snake
 *    (substituir Program.cs por este arquivo)
 *    dotnet run
 * ============================================================
 */

using System;
using System.Collections.Generic;
using System.Threading;

class SnakeGame
{
    // ── Constantes do tabuleiro ──────────────────────────────
    const int LARGURA  = 40;   // colunas internas do tabuleiro
    const int ALTURA   = 20;   // linhas internas do tabuleiro

    // ── Símbolos visuais ─────────────────────────────────────
    const char SIMBOLO_CABECA  = 'O';
    const char SIMBOLO_CORPO   = 'o';
    const char SIMBOLO_FRUTA   = '*';   // ★ pode não renderizar em todos os consoles
    const char SIMBOLO_BORDA_H = '═';
    const char SIMBOLO_BORDA_V = '║';
    const char SIMBOLO_CANTO   = '+';

    // ── Direções possíveis ───────────────────────────────────
    enum Direcao { Cima, Baixo, Esquerda, Direita }

    // ── Estado global do jogo ────────────────────────────────
    static List<(int x, int y)> cobra = new List<(int, int)>();
    static Direcao direcaoAtual   = Direcao.Direita;
    static Direcao proximaDirecao = Direcao.Direita;
    static (int x, int y) fruta;
    static int pontuacao  = 0;
    static bool jogoAtivo = true;
    static Random aleatorio = new Random();

    // ── Velocidade base em ms (diminui com a pontuação) ──────
    static int velocidade = 150;

    // ════════════════════════════════════════════════════════
    //  PONTO DE ENTRADA
    // ════════════════════════════════════════════════════════
    static void Main()
    {
        ConfigurarConsole();
        Inicializar();
        DesenharTabuleiro();

        // ── WHILE: loop principal — roda enquanto o jogo estiver ativo ──
        while (jogoAtivo)
        {
            LerTecla();          // captura input sem bloquear
            MoverCobra();        // atualiza posição
            VerificarColisao();  // checa parede, corpo e fruta
            DesenharCobra();     // redesenha só o necessário
            AtualizarPlacar();   // exibe pontuação

            Thread.Sleep(velocidade); // controla a velocidade do jogo
        }

        TelaGameOver();
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 1 — Configurar console
    // ════════════════════════════════════════════════════════
    static void ConfigurarConsole()
    {
        Console.CursorVisible = false;        // esconde o cursor piscante
        Console.Title = "🐍 Snake Game — UC1";

        // Tenta redimensionar a janela (funciona no Windows)
        try
        {
            Console.SetWindowSize(LARGURA + 4, ALTURA + 6);
            Console.SetBufferSize(LARGURA + 4, ALTURA + 6);
        }
        catch { /* ignora em sistemas que não suportam */ }

        Console.Clear();
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 2 — Inicializar estado do jogo
    // ════════════════════════════════════════════════════════
    static void Inicializar()
    {
        cobra.Clear();
        pontuacao     = 0;
        jogoAtivo     = true;
        velocidade    = 150;
        direcaoAtual  = Direcao.Direita;
        proximaDirecao = Direcao.Direita;

        // Cobra começa com 3 segmentos no centro do tabuleiro
        int centroX = LARGURA / 2;
        int centroY = ALTURA  / 2;

        cobra.Add((centroX,     centroY));   // cabeça
        cobra.Add((centroX - 1, centroY));   // corpo
        cobra.Add((centroX - 2, centroY));   // rabo

        GerarFruta(); // posiciona a primeira fruta
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 3 — Desenhar o tabuleiro (bordas)
    // ════════════════════════════════════════════════════════
    static void DesenharTabuleiro()
    {
        Console.Clear();

        // ── FOR: percorre cada coluna para desenhar a linha superior ──
        Console.SetCursorPosition(0, 0);
        Console.Write(SIMBOLO_CANTO);
        for (int x = 0; x < LARGURA; x++)
            Console.Write(SIMBOLO_BORDA_H);
        Console.Write(SIMBOLO_CANTO);

        // ── FOR: percorre cada linha lateral ──
        for (int y = 1; y <= ALTURA; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(SIMBOLO_BORDA_V);
            Console.SetCursorPosition(LARGURA + 1, y);
            Console.Write(SIMBOLO_BORDA_V);
        }

        // Linha inferior
        Console.SetCursorPosition(0, ALTURA + 1);
        Console.Write(SIMBOLO_CANTO);
        for (int x = 0; x < LARGURA; x++)
            Console.Write(SIMBOLO_BORDA_H);
        Console.Write(SIMBOLO_CANTO);

        // Cabeçalho com instruções
        Console.SetCursorPosition(0, ALTURA + 3);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  Setas: mover   |   ESC: sair");
        Console.ResetColor();

        // Desenha a cobra inicial e a fruta
        DesenharCobra();
        DesenharPosicao(fruta.x, fruta.y, SIMBOLO_FRUTA, ConsoleColor.Yellow);
        AtualizarPlacar();
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 4 — Gerar fruta em posição aleatória
    // ════════════════════════════════════════════════════════
    static void GerarFruta()
    {
        // ── IF: garante que a fruta não apareça dentro da cobra ──
        do
        {
            fruta.x = aleatorio.Next(0, LARGURA);
            fruta.y = aleatorio.Next(0, ALTURA);
        }
        while (cobra.Contains(fruta));

        DesenharPosicao(fruta.x, fruta.y, SIMBOLO_FRUTA, ConsoleColor.Yellow);
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 5 — Ler tecla pressionada (sem travar)
    // ════════════════════════════════════════════════════════
    static void LerTecla()
    {
        // Console.KeyAvailable: verifica se há tecla sem bloquear o jogo
        if (!Console.KeyAvailable) return;

        ConsoleKeyInfo tecla = Console.ReadKey(true); // true = não exibe na tela

        // ── SWITCH/CASE: processa a tecla para mudar a direção ──
        switch (tecla.Key)
        {
            case ConsoleKey.UpArrow:
                // IF: impede inversão de 180° (não pode ir para cima se está indo para baixo)
                if (direcaoAtual != Direcao.Baixo)
                    proximaDirecao = Direcao.Cima;
                break;

            case ConsoleKey.DownArrow:
                if (direcaoAtual != Direcao.Cima)
                    proximaDirecao = Direcao.Baixo;
                break;

            case ConsoleKey.LeftArrow:
                if (direcaoAtual != Direcao.Direita)
                    proximaDirecao = Direcao.Esquerda;
                break;

            case ConsoleKey.RightArrow:
                if (direcaoAtual != Direcao.Esquerda)
                    proximaDirecao = Direcao.Direita;
                break;

            case ConsoleKey.Escape:
                jogoAtivo = false; // ESC encerra o jogo
                break;
        }
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 6 — Mover a cobra
    // ════════════════════════════════════════════════════════
    static void MoverCobra()
    {
        direcaoAtual = proximaDirecao;

        // Calcula nova posição da cabeça com base na direção
        var (cx, cy) = cobra[0];
        int novoX = cx;
        int novoY = cy;

        // ── SWITCH/CASE: calcula deslocamento conforme direção ──
        switch (direcaoAtual)
        {
            case Direcao.Cima:     novoY--; break;
            case Direcao.Baixo:    novoY++; break;
            case Direcao.Esquerda: novoX--; break;
            case Direcao.Direita:  novoX++; break;
        }

        // Apaga a última parte do rabo antes de adicionar nova cabeça
        var rabo = cobra[cobra.Count - 1];
        LimparPosicao(rabo.x, rabo.y);

        // ── FOR: empurra o corpo (cada segmento assume posição do anterior) ──
        for (int i = cobra.Count - 1; i > 0; i--)
            cobra[i] = cobra[i - 1];

        // Nova cabeça na posição calculada
        cobra[0] = (novoX, novoY);
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 7 — Verificar colisões
    // ════════════════════════════════════════════════════════
    static void VerificarColisao()
    {
        var (hx, hy) = cobra[0]; // posição da cabeça

        // ── IF: colisão com as paredes ──
        if (hx < 0 || hx >= LARGURA || hy < 0 || hy >= ALTURA)
        {
            jogoAtivo = false;
            return;
        }

        // ── FOR + IF: colisão com o próprio corpo ──
        for (int i = 1; i < cobra.Count; i++)
        {
            if (cobra[i].x == hx && cobra[i].y == hy)
            {
                jogoAtivo = false;
                return;
            }
        }

        // ── IF: cobra comeu a fruta ──
        if (hx == fruta.x && hy == fruta.y)
        {
            pontuacao += 10;

            // Cresce: adiciona um segmento extra no rabo
            cobra.Add(cobra[cobra.Count - 1]);

            // Aumenta velocidade a cada 30 pontos (mínimo 50ms)
            if (pontuacao % 30 == 0 && velocidade > 50)
                velocidade -= 10;

            GerarFruta(); // gera nova fruta
        }
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 8 — Desenhar a cobra no console
    // ════════════════════════════════════════════════════════
    static void DesenharCobra()
    {
        // ── FOR: percorre todos os segmentos da cobra ──
        for (int i = 0; i < cobra.Count; i++)
        {
            // ── IF/ELSE: cabeça tem cor e símbolo diferente do corpo ──
            if (i == 0)
                DesenharPosicao(cobra[i].x, cobra[i].y, SIMBOLO_CABECA, ConsoleColor.Green);
            else
                DesenharPosicao(cobra[i].x, cobra[i].y, SIMBOLO_CORPO, ConsoleColor.DarkGreen);
        }
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 9 — Atualizar placar
    // ════════════════════════════════════════════════════════
    static void AtualizarPlacar()
    {
        Console.SetCursorPosition(0, ALTURA + 2);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"  Pontuação: {pontuacao,5}   Tamanho: {cobra.Count,3}   Vel: {(200 - velocidade) / 10 + 1,2}x  ");
        Console.ResetColor();
    }

    // ════════════════════════════════════════════════════════
    //  FUNÇÃO 10 — Tela de Game Over
    // ════════════════════════════════════════════════════════
    static void TelaGameOver()
    {
        // Centraliza a mensagem no tabuleiro
        int midX = LARGURA / 2 - 8;
        int midY = ALTURA  / 2 - 1;

        Console.SetCursorPosition(midX, midY + 1);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("  ╔══════════════════╗  ");

        Console.SetCursorPosition(midX, midY + 2);
        Console.Write("  ║   GAME  OVER!   ║  ");

        Console.SetCursorPosition(midX, midY + 3);
        Console.Write("  ╚══════════════════╝  ");
        Console.ResetColor();

        Console.SetCursorPosition(midX, midY + 5);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"  Pontuação final: {pontuacao}");

        Console.SetCursorPosition(midX, midY + 6);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  [R] Jogar de novo   [ESC] Sair");
        Console.ResetColor();

        // Aguarda o jogador decidir
        while (true)
        {
            var tecla = Console.ReadKey(true);

            // ── IF/ELSE: R reinicia, ESC sai ──
            if (tecla.Key == ConsoleKey.R)
            {
                Inicializar();
                DesenharTabuleiro();
                jogoAtivo = true;

                // Volta ao loop principal
                while (jogoAtivo)
                {
                    LerTecla();
                    MoverCobra();
                    VerificarColisao();
                    DesenharCobra();
                    AtualizarPlacar();
                    Thread.Sleep(velocidade);
                }

                TelaGameOver(); // recursão para nova tela de game over
                return;
            }
            else if (tecla.Key == ConsoleKey.Escape)
            {
                Console.SetCursorPosition(0, ALTURA + 5);
                Console.WriteLine("\n  Até logo! 🐍\n");
                return;
            }
        }
    }

    // ════════════════════════════════════════════════════════
    //  AUXILIARES — Posicionar cursor e desenhar / apagar char
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// Posiciona o cursor e escreve um caractere colorido.
    /// Offset de +1 em x e y por causa das bordas.
    /// </summary>
    static void DesenharPosicao(int x, int y, char simbolo, ConsoleColor cor)
    {
        Console.SetCursorPosition(x + 1, y + 1); // +1 compensa a borda
        Console.ForegroundColor = cor;
        Console.Write(simbolo);
        Console.ResetColor();
    }

    /// <summary>
    /// Apaga um caractere (substitui por espaço).
    /// </summary>
    static void LimparPosicao(int x, int y)
    {
        Console.SetCursorPosition(x + 1, y + 1);
        Console.Write(' ');
    }
}