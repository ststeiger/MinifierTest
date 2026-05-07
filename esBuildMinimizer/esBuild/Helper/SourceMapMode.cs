
namespace esBuildMinimizer
{
    public enum SourceMapMode
    {
        None, // no source map at all 
        Inline, // source map appended as base64 at end of file 
        Linked, // reference to source map file added at end of file 
        External, // with source map file, but no reference added at end of file 
        Both // with source map file, and source map also appended as base64 at end of file 
    } // End Enum SourceMapMode 

} // End Namespace 
