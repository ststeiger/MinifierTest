
namespace esBuildMinimizer
{


    public static class DevelopmentHelper 
    {


        public static string ProjectDirectory
        {
            get
            {
                if (System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", System.StringComparison.OrdinalIgnoreCase))
                    return System.IO.Path.GetFullPath(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", ".."));

                string bd = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
                return bd;
            } // End Getter 
        } // End Property ProjectDirectory 


    } // End Class DevelopmentHelper 


} // End Namespace 
