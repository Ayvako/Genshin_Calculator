namespace Genshin_Calculator.Helpers
{
    public static class ResourcePaths
    {
        private static string? basePath;

        public static string Characters => $"{BasePath}/Characters";

        public static string Materials => $"{BasePath}/Materials";

        public static string Tools => $"{BasePath}/Tools";

        private static string BasePath => basePath;

        public static void SetBasePath(string path)
        {
            basePath = path;
        }
    }
}