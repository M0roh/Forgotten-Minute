public class ShopState
{
    private Item _item;
    private int _slot;
    private bool _isBuyed = false;

    public Item ShopItem => _item;
    public int Slot => _slot;
    public bool IsBuyed => _isBuyed;

    public ShopState(Item item, int slot)
    {
        _slot = slot;

        _item = item;
    }

    public void Item_OnBuy()
    {
        _isBuyed = true;
    }
}
