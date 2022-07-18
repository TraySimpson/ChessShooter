using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorStore {
    private Dictionary<int, List<int>> factorDictionary;

    public FactorStore() {
        factorDictionary = new Dictionary<int, List<int>>();
    }

    public List<int> GetFactors(int number) {
        if (!factorDictionary.ContainsKey(number))
            BuildFactorEntries(number);
        return factorDictionary[number];
    }

    private void BuildFactorEntries(int number) {
        List<int> factors = new List<int>();
        for (int i = 2; i < number; i++) {
            if (number % i == 0)
                factors.Add(i);
        }
        factorDictionary.Add(number, factors);
    }
}
