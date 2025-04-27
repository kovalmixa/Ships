using System.Collections.Generic;
using System.Dynamic;

public class Route
{
    public List<IScript> Areas = new();
    private int _currentIndex = 0;
    public IScript GetCurrentScript()
    {
        if (_currentIndex >= Areas.Count)
            return null;
        return Areas[_currentIndex];
    }

    public void MoveNext() => _currentIndex++;
    public void SetIndex(int index) => _currentIndex = index;
}
