/// <summary>
/// Codes the program can return after execution has finished
/// </summary>
public enum ReturnCode
{
    /// <summary>
    /// No errors were encountered
    /// </summary>
    Success = 0,
    /// <summary>
    /// An error happened that caused an exception in the code
    /// </summary>
    ProcessError = 1,
    /// <summary>
    /// Problems were found in the data
    /// </summary>
    DataError = 2
}