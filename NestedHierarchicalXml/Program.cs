
using System.Windows.Forms;


namespace NestedHierarchicalXml
{


    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
            bool is_console_app = System.Console.OpenStandardInput(1) != System.IO.Stream.Null;

            if (!is_console_app)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            } // End if (!is_console_app)



            GroupedExport.ExportXML();

            if (is_console_app)
            {
                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine(" --- Press any key to continue --- ");
                System.Console.ReadKey();
            } // End if (is_console_app)

        } // End Sub Main


    } // End Class Program


} // End Namespace NestedHierarchicalXml
