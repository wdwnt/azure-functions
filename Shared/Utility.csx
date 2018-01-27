public static class Utility
{
    public static string GetEnvironmentVariable(string variableName)
    {
        return System.Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process);
    }
}
