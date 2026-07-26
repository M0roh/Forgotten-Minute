using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _wallPart;
    [SerializeField] private GameObject _doorPart;

    private bool _isOpened = true;
    private bool _isExists = true;

    public bool IsOpen => _isOpened && _isExists;
    public bool IsExists => _isExists;

    public void Delete()
    {
        if (!_isExists) return;

        _isExists = false;
        _wallPart.SetActive(true);
        _doorPart.SetActive(false);
    }

    public void Create()
    {
        if (_isExists) return;

        _isExists = true;
        _wallPart.SetActive(false);
        _doorPart.SetActive(true);
    }

    public void Open()
    {
        _isOpened = true;
        _doorPart.GetComponent<Collider2D>().isTrigger = true;
    }
    public void Close()
    {
        _isOpened = false;
        _doorPart.GetComponent<Collider2D>().isTrigger = false;
    }
}
