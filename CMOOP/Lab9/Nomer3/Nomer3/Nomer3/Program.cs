using Nomer3;

public class Program
{
    public static void Main()
    {
        ArmoryStore myStore = new ArmoryStore(10);

        myStore.AddItem(new Item("HP Potion", ItemCategory.Consumable, 50, 10.5m));
        myStore.AddItem(new Item("Iron Sword", ItemCategory.Weapon, 5, 150.0m));
        myStore.AddItem(new Item("Mana Scroll", ItemCategory.Consumable, 20, 25.0m));

        myStore.ShowInventory();

        myStore.UpdateStock("HP Potion", 100, 12.0m);

        myStore.RemoveItem("Iron Sword");

        myStore.ShowInventory();
    }
}