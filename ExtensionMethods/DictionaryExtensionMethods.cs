namespace MyPosTest.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> pDictionary, TKey pKey, TValue pValue)
        {
            if (pDictionary == null) pDictionary = new Dictionary<TKey, TValue>();

            if (pDictionary.ContainsKey(pKey))
                pDictionary[pKey] = pValue;
            else
                pDictionary.Add(pKey, pValue);

            return pDictionary;
        }

        public static bool Contains<TKey, TValue>(this Dictionary<TKey, TValue> pDictionary, TKey pKey, TValue pValue)
        {
            if (pDictionary == null) return false;
            if (!pDictionary.ContainsKey(pKey)) return false;

            if (pDictionary[pKey] == null && pValue == null) return true;
            if (pDictionary[pKey] == null && pValue != null)
                return false;


            if (pValue == null && pDictionary[pKey] != null)
                return false;


            return pDictionary[pKey].Equals(pValue);
        }
    }
}
