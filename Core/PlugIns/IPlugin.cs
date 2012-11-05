namespace ResEx.Core.PlugIns
{
    public interface IPlugIn
    {
        /// <summary>
        /// Initializes the plug in. This is executed upon application start-up
        /// </summary>
        /// <param name="context">A reference to the context of the application</param>
        void Initialize(IContext context);
    }
}