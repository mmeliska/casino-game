namespace Starflare.State
{
    /// <summary>
    /// Where a game module hands control back: the only moments a story beat or seed reveal may
    /// fire. A table raises <c>BreakPointReached</c> with one of these; it never plays story itself.
    /// </summary>
    public enum BreakPoint { GotUpFromTable }
}
