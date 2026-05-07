
namespace esBuildMinimizer
{
    public sealed class ESBuildResult
    {
        public string JavaScript { get; set; }
        public string SourceMap { get; set; }


        public ESBuildResult(
            string javaScript, 
            string sourceMap
        )
        {
            JavaScript = javaScript;
            SourceMap = sourceMap;
        } // End Constructor 


    } // End Class ESBuildResult 


} // End Namespace 
