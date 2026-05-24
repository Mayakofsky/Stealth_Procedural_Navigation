using UnityEngine;

// 1. КЛАСС УЗЛА ДЕРЕВА (Аналог структуры-узла в C++)
public class bspRoomNode
{
    public int x, y;
    public int width, height;

    public bspRoomNode leftChild;  // В C++ это было бы: bspRoomNode* leftChild;
    public bspRoomNode rightChild; // В C++ это было бы: bspRoomNode* rightChild;

    public bspRoomNode(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public bool IsLeaf()
    {
        return leftChild == null && rightChild == null;
    }

    // НАША НОВАЯ ФУНКЦИЯ РАЗБИЕНИЯ
    // Метод возвращает true, если узел успешно разрезали, и false, если он уже слишком мал
    public bool Split(int minRoomSize)
    {
        if (!IsLeaf()) return false;

        // true - горизонтально, false - вертикально
        bool splitHorizontal = Random.Range(0f, 1f) > 0.5f;

        if (width > height && (float)width / height >= 1.25f) splitHorizontal = false;
        else if (height > width && (float)height / width >= 1.25f) splitHorizontal = true;

        int maxSplit = (splitHorizontal ? height : width) - minRoomSize;

        if (maxSplit < minRoomSize) return false;

        int splitPoint = Random.Range(minRoomSize, maxSplit);

        // Создаем двух новых потомков в куче (выделение памяти, в C++ это был бы new bspRoomNode(...))
        if (splitHorizontal)
        {
            leftChild = new bspRoomNode(x, y, width, splitPoint);
            rightChild = new bspRoomNode(x, y + splitPoint, width, height - splitPoint);
        }
        else
        {
            leftChild = new bspRoomNode(x, y, splitPoint, height);
            rightChild = new bspRoomNode(x + splitPoint, y, width - splitPoint, height);
        }

        return true; // Разрез прошел успешно
    }
}


public class BSPTree : MonoBehaviour
{
    [Header("Настройки карты")]
    public int mapWidth = 60;
    public int mapHeight = 60;
    public int minRoomSize = 10;

    private bspRoomNode rootNode;

    void Start()
    {
        rootNode = new bspRoomNode(0, 0, mapWidth, mapHeight);
        GenerateBSP(rootNode);
        Debug.Log("BSP Дерево успешно построено!");
    }

    void GenerateBSP(bspRoomNode currentNode)
    {
        if (currentNode == null) return;

        if (currentNode.Split(minRoomSize))
        {
            GenerateBSP(currentNode.leftChild);

            GenerateBSP(currentNode.rightChild);
        }
    }
   // метод отладочной отрисовки
    void OnDrawGizmos()
    {
        if (rootNode == null) return;
        DrawNodeGizmos(rootNode);
    }

    void DrawNodeGizmos(bspRoomNode currentNode)
    {
        if (currentNode == null) return;

        if (currentNode.IsLeaf())
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3(currentNode.x + currentNode.width / 2f, currentNode.y + currentNode.height / 2f, 0);
            Vector3 size = new Vector3(currentNode.width, currentNode.height, 0);
            Gizmos.DrawWireCube(center, size);
        }

        DrawNodeGizmos(currentNode.leftChild);
        DrawNodeGizmos(currentNode.rightChild);
    }
}