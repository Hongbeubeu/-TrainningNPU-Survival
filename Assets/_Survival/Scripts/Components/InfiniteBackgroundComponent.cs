using UnityEngine;

public class InfiniteBackgroundComponent : MonoBehaviour
{
    public float Size = 100;
    public BackgroundItemComponent[] BackgroundItemComponents;
    public Vector2 CurrentMiddle;
    
    private void Awake()
    {
        CurrentMiddle = Vector2.zero;
        SetInfo();
    }
    
    public void SetInfo()
    {
        var grid = CurrentMiddle;
        var count = 0;
        for (var i = -1; i < 2; i++)
        {
            for (var j = -1; j < 2; j++)
            {
                grid.x = CurrentMiddle.x + i;
                grid.y = CurrentMiddle.y + j;
                BackgroundItemComponents[count].SetPosition(GridPos2WorldPos(grid));
                count++;
            }
        }
    }
    
    public void SetMiddle(Vector2 worldPos)
    {
        var newMiddle = WordPos2GridPos(worldPos);
        if (newMiddle == CurrentMiddle)
            return;
        CurrentMiddle = newMiddle;
        SetInfo();
    }
    
    public Vector2 GridPos2WorldPos(Vector2 gridPos)
    {
        return gridPos * Size;
    }
    
    public Vector2 WordPos2GridPos(Vector2 worldPos)
    {
        var x = (int)(Mathf.Abs(worldPos.x) / Size + 0.5f);
        if (worldPos.x < 0)
            x = -x;
        var y = (int)(Mathf.Abs(worldPos.y) / Size + 0.5f);
        if (worldPos.y < 0)
            y = -y;
        return new Vector2(x, y);
    }
}