// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("qYVc11v0B3QUe6GX3tngz/pN39u3vYThcP8c9qLpYQjiIyMmYkpfJVg72aNjEhFleGkvCrcDwr9uj+8QF8e64eLFZ2vvyTqEHiYoyvVHrHB7EmMPfSz9tXuge9IWmcah5+lLa47WPpFZssQC9YGEg5OqETUiFVIZglwdbGGn1UvlorN1Nhv0pL7VpSf9gJ5YO6Ya0owE85S79PyN2u7VdMdvEtVY6JpdzhUo9LiTyq1OlQpvgazN/KSranrLdVwfAZhUH33njEpe7G9MXmNoZ0ToJuiZY29vb2tubexvYW5e7G9kbOxvb26/Yedw0B/BFOil1ig4wznwRXC+RuEv6uvGG6lELzvAAfoBERZ4LhueNkVLwW2Yzxa7M688yPFwQ2xtb25v");
        private static int[] order = new int[] { 9,7,5,12,13,6,8,10,13,11,11,11,12,13,14 };
        private static int key = 110;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
