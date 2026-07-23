using UnityEngine;

public class MessageTrigger : ColliderTrigger
{
    [SerializeField] private string _messageOnEnter;
    [SerializeField] private string _messageOnStay;
    [SerializeField] private string _messageOnExit;

    private void Awake()
    {
        _onEnter.AddListener(() => SendMessage(_messageOnEnter));
        _onStay.AddListener(() => SendMessage(_messageOnStay));
        _onExit.AddListener(() => SendMessage(_messageOnExit));
    }

    public void SendMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        //GameManager.Instance.SendMessage(message).Forget();
    }
}
