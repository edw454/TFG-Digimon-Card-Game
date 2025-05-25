using Fusion;

public struct CardData : INetworkStruct
{
    public NetworkString<_16> Code;
    public NetworkString<_16> NameByte;  // En lugar de un string, usa un �ndice
    public int Dp;
    public int Cost;

    public override bool Equals(object obj)
    {
        if (!(obj is CardData)) return false;
        CardData other = (CardData)obj;
        // Compara todas las propiedades relevantes
        return this.Code == other.Code && this.NameByte == other.NameByte; // Ejemplo
    }

    public override int GetHashCode()
    {
        unchecked // Overflow est� permitido
        {
            int hash = 17;
            hash = hash * 23 + Code.GetHashCode(); // NetworkString es struct, no necesita null-check
            hash = hash * 23 + NameByte.GetHashCode();
            return hash;
        }
    }
}

