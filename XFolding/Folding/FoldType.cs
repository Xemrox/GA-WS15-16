namespace XGA {

    // 0 = hydrophil, "white"
    // 1 = hydrophob, "black"
    public enum FoldType {
        Hydrophilic = 0,
        Hydrophobic = 1
    }

    public static class FoldTypeExt {

        public static string Print(this FoldType elem) {
            return elem == FoldType.Hydrophilic ? "1" : "0";
        }
    }
}