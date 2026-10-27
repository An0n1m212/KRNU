namespace TestingSystem.Views;

/// <summary>Пара «значение перечисления — локализованный текст» для ComboBox.</summary>
public record EnumItem<T>(T Value, string Text) where T : struct, Enum
{
    public override string ToString() => Text;
}
