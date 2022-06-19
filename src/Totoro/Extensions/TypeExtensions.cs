namespace Totoro.Extensions;

public static class TypeExtensions
{
    public static bool IsSubclassOfGeneric(this Type type, Type generic)
    {
        var except = typeof(object);

        while (type != except)
        {
            var definition = type.IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (generic == definition)
            {
                return true;
            }

            if (type.BaseType == null)
            {
                return false;
            }

            type = type.BaseType;
        }

        return false;
    }
}
