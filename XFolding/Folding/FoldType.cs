namespace XGA {

    public enum FoldType {
        Hydrophilic,
        Hydrophobic
    }

    public static class FoldTypeExt {

        public static string Print(this FoldType elem) {
            return elem == FoldType.Hydrophilic ? "0" : "1";
        }
    }
}