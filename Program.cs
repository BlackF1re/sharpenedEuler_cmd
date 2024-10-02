class Program
{
    static List<List<int>> ReadAdjacencyMatrix(string filename)
    {
        return File.ReadAllLines(filename)
                   .Select(line => line.Split(' ').Select(int.Parse).ToList())
                   .ToList();
    }

    static bool IsConnected(List<List<int>> adjMatrix)
    {
        int n = adjMatrix.Count;
        bool[] visited = new bool[n];

        void Dfs(int v)
        {
            visited[v] = true;
            for (int u = 0; u < n; u++)
            {
                if (adjMatrix[v][u] > 0 && !visited[u])
                {
                    Dfs(u);
                }
            }
        }

        Dfs(0);
        return visited.All(v => v);
    }

    static bool AllVerticesEvenDegree(List<List<int>> adjMatrix)
    {
        return adjMatrix.All(row => row.Sum() % 2 == 0);
    }

    static List<int> FindEulerCycle(List<List<int>> adjMatrix)
    {
        int n = adjMatrix.Count;
        List<int> path = new List<int>();

        void Dfs(int v)
        {
            for (int u = 0; u < n; u++)
            {
                while (adjMatrix[v][u] > 0)
                {
                    adjMatrix[v][u]--;
                    adjMatrix[u][v]--;
                    Dfs(u);
                }
            }
            path.Add(v);
        }

        for (int start = 0; start < n; start++)
        {
            if (adjMatrix[start].Sum() > 0)
            {
                Dfs(start);
                break;
            }
        }

        path.Reverse();
        return path;
    }

    static void Main()
    {
        string matricesDir = "matrices";
        var files = Directory.GetFiles(matricesDir, "*.txt").Select(Path.GetFileName).ToList();

        if (!files.Any())
        {
            Console.WriteLine("Не обнаружено .txt файлов в директории matrices.");
            return;
        }

        Console.WriteLine("Доступные файлы матриц смежности:");
        for (int i = 0; i < files.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {files[i]}");
        }
        Console.Write("Введите номер файла с матрицей смежности:\t");
        int choice = int.Parse(Console.ReadLine()) - 1;

        if (choice < 0 || choice >= files.Count)
        {
            Console.WriteLine("Неверный выбор.");
            return;
        }

        string filename = Path.Combine(matricesDir, files[choice]);

        List<List<int>> adjMatrix;
        try
        {
            adjMatrix = ReadAdjacencyMatrix(filename);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка чтения файла: {e.Message}");
            return;
        }

        if (!IsConnected(adjMatrix))
        {
            Console.WriteLine("Граф не связный.");
            return;
        }

        if (!AllVerticesEvenDegree(adjMatrix))
        {
            for (int i = 0; i < adjMatrix.Count; i++)
            {
                Console.WriteLine($"Вершина {i + 1} имеет степень {adjMatrix[i].Sum()}");
            }
            Console.WriteLine("Не все вершины имеют четную степень.");
            return;
        }

        Console.WriteLine("Граф связный и все вершины имеют четные степени. Вычисление...");

        var adjMatrixCopy = adjMatrix.Select(row => new List<int>(row)).ToList();
        var eulerCycle = FindEulerCycle(adjMatrixCopy);

        if (eulerCycle.Count == 1) Console.WriteLine("Эйлеров цикл не найден на графе.");
        else Console.WriteLine($"Эйлеров цикл найден: {string.Join(" -> ", eulerCycle.Select(v => v + 1))}");
    }
}