
namespace NestedHierarchicalXml
{


    static class GroupedExport
    {

        public static void ExportXML()
        {
            string FileName = @"d:\test.xml";
            ExportXML(FileName);
        }


        // NestedHierarchicalXml.GroupedExport.ExportXML();
        public static void ExportXML(string FileName)
        {
            bool asAttributes = false;
            bool bRemoveUnwantedElementsOrValues = true;

            string strSQL = @"
      SELECT 9999 AS i, NULL AS InternalName, 'RoomExport' AS DisplayName 
UNION SELECT 1 AS i, 'T_Room' AS InternalName, 'Room' AS DisplayName 
UNION SELECT 2 AS i, 'Workplaces' AS InternalName, 'Workplaces' AS DisplayName 
UNION SELECT 3 AS i, 'T_Workplace' AS InternalName, 'Workplace' AS DisplayName 
UNION SELECT 4 AS i, 'WorksOfArt' AS InternalName, 'WorksOfArt' AS DisplayName 
UNION SELECT 5 AS i, 'T_Art' AS InternalName, 'ArtWork' AS DisplayName 
;


SELECT null as bla, * FROM T_Room; 
SELECT DISTINCT WP_RM_UID FROM T_Workplace AS Workplaces; 
SELECT * FROM T_Workplace WHERE WP_RM_UID IS NOT NULL; 
SELECT DISTINCT AR_RM_UID FROM T_Art AS WorksOfArt; 
SELECT * FROM T_Art WHERE AR_RM_UID IS NOT NULL; 
";

            using (System.Data.DataSet ds = new System.Data.DataSet())
            {

                using (System.Data.Common.DbDataAdapter da = GetDataAdapter(strSQL))
                {
                    da.Fill(ds);
                } // End Using da


                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    int i = System.Convert.ToInt32(dr["i"]);
                    string internalName = System.Convert.ToString(dr["InternalName"]);

                    if (i == 9999)
                        continue;

                    ds.Tables[i].TableName = internalName;
                } // Next dr 

                


                // http://www.tomasvera.com/programming/generating-hierarchical-nested-xml-from-a-dataset/

                // ---------------------- Workplaces

                System.Data.DataRelation relRoomWorkplaces = new System.Data.DataRelation(
                    "relRoomWorkplaces" // relation name
                    , ds.Tables["T_Room"].Columns["RM_UID"] // parent column
                    , ds.Tables["Workplaces"].Columns["WP_RM_UID"] // child column
                );

                relRoomWorkplaces.Nested = true;


                // This relation will yield an "Orders" node in each Customer node
                System.Data.DataRelation relWorkplaces = new System.Data.DataRelation(
                    "relWorkplaces" // relation name
                    , ds.Tables["Workplaces"].Columns["WP_RM_UID"] // parent column
                    , ds.Tables["T_Workplace"].Columns["WP_RM_UID"] // child column
                );
                relWorkplaces.Nested = true;


                // ---------------------- Art 

                System.Data.DataRelation relRoomWorksOfArt = new System.Data.DataRelation(
    "relRoomWorksOfArt" // relation name
    , ds.Tables["T_Room"].Columns["RM_UID"] // parent column
    , ds.Tables["WorksOfArt"].Columns["AR_RM_UID"] // child column
);

                relRoomWorksOfArt.Nested = true;


                // This relation will yield an "Orders" node in each Customer node
                System.Data.DataRelation relArt = new System.Data.DataRelation(
                    "relArt" // relation name
                    , ds.Tables["WorksOfArt"].Columns["AR_RM_UID"] // parent column
                    , ds.Tables["T_Art"].Columns["AR_RM_UID"] // child column
                );
                relArt.Nested = true;




                // relRoomWorkplaces.Nested = true;
                ds.Relations.Add(relRoomWorkplaces);
                ds.Relations.Add(relWorkplaces);
                ds.Relations.Add(relRoomWorksOfArt);
                ds.Relations.Add(relArt);



                // Finished adding relations
                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    int i = System.Convert.ToInt32(dr["i"]);
                    string displayName = System.Convert.ToString(dr["DisplayName"]);

                    if(i==9999)
                        ds.DataSetName = displayName;
                    else
                        ds.Tables[i].TableName = displayName;
                } // Next dr 

                

                ds.Tables.Remove(ds.Tables[0]);

                if (asAttributes)
                {
                    foreach (System.Data.DataTable dt in ds.Tables)
                    {

                        foreach (System.Data.DataColumn dc in dt.Columns)
                        {
                            dc.ColumnMapping = System.Data.MappingType.Attribute;
                            dc.DefaultValue = null;
                            // if(object.ReferenceEquals(dc.DataType, typeof(string))) dc.DefaultValue = String.Empty; else if (object.ReferenceEquals(dc.DataType, typeof(Guid))) dc.DefaultValue = Guid.Empty;
                        } // Next dc 

                    } // Next dt 

                } // End if (asAttributes)



                System.Xml.XmlDocument doc = RemoveUnwantedElementsOrValues(ds, bRemoveUnwantedElementsOrValues);

                System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
                settings.Indent = true;
                //settings.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1")
                settings.Encoding = System.Text.Encoding.UTF8;
                settings.CloseOutput = true;
                settings.CheckCharacters = true;
                settings.NewLineChars = System.Environment.NewLine;
                settings.OmitXmlDeclaration = false;


                // Write as UTF-8 with indentation
                using (System.Xml.XmlWriter w = System.Xml.XmlWriter.Create(FileName, settings)) 
                {
                    if (doc != null)
                        doc.Save(w);
                    else
                        ds.WriteXml(w);
                } // End Using w

            } // End Using ds 

        } // End Sub ExportXML


        public static System.Xml.XmlDocument RemoveUnwantedElementsOrValues(System.Data.DataSet ds, bool bRemoveUnwantedElementsOrValues)
        {
            if (bRemoveUnwantedElementsOrValues)
                return null;


            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            
            string strXML = ds.GetXml();
            doc.LoadXml(strXML);


            System.Xml.XmlNamespaceManager names = new System.Xml.XmlNamespaceManager(doc.NameTable);
            names.AddNamespace("dft", "");

            string[] selectors = new string[] { 
                     "//Workplaces/WP_RM_UID" 
                    ,"//WorksOfArt/AR_RM_UID" 
            };


            foreach (string selector in selectors)
            {
                System.Xml.XmlNodeList nodeList = doc.SelectNodes(selector, names);

                if (nodeList != null)
                {
                    foreach (System.Xml.XmlNode thisNode in nodeList)
                    {
                        if (thisNode.ParentNode != null)
                            thisNode.ParentNode.RemoveChild(thisNode);
                    } // Next thisNode

                } // End if (nodeList != null)

            } // Next selector


            selectors = new string[] { 
                     "//Workplaces[@WP_RM_UID]"
                    ,"//WorksOfArt[@AR_RM_UID]"
            };


            foreach (string selector in selectors)
            {
                System.Xml.XmlNodeList nodeList = doc.SelectNodes(selector, names);
                string attributeName = System.Text.RegularExpressions.Regex.Match(selector, @"\[@(.*)\]").Groups[1].Value;

                if (nodeList != null)
                {
                    foreach (System.Xml.XmlNode thisNode in nodeList)
                    {
                        if (thisNode.Attributes != null)
                            thisNode.Attributes.RemoveNamedItem(attributeName);
                    } // Next thisNode
                } // End if (nodeList != null)

            } // Next selector

            // doc.Save(@"d:\test2.xml");
            return doc;
        } // End Function RemoveUnwantedElementsOrValues 


        public static void ChangeStandalone(string loadPath, string savePath, System.Xml.XmlWriterSettings settings)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(loadPath);
            // doc.LoadXml(strXML);

            System.Xml.XmlElement element = doc.DocumentElement;

            // http://xmlwriter.net/xml_guide/xml_declaration.shtml
            // System.Xml.XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", settings.Encoding.WebName, "yes");
            // System.Xml.XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", settings.Encoding.WebName, "no");
            System.Xml.XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", settings.Encoding.WebName, null);
            doc.ReplaceChild(xmlDeclaration, doc.FirstChild);

            // If the XML declaration is included, it must be situated at the first position of the first line in the XML document.
            // If the XML declaration is included, it must contain the version number attribute .
            // If all of the attributesglossary are declared in an XML declaration, they must be placed in the order shown above.
            // If any elements, attributes, or entities are used in the XML document that are referenced or defined in an external DTD, standalone="no" must be included.
            // The XML declaration must be in lower case (except for the encoding declarations).
            // Note: The XML declaration has no closing tag, that is </?xml>


            // Rename the document-root
            // doc.DocumentElement.Name = "lalala";
            //System.Xml.XmlElement objNewRoot = doc.CreateElement("MasterList");
            //objNewRoot.InnerXml = doc.DocumentElement.InnerXml;
            //doc.RemoveChild(doc.DocumentElement);
            //doc.AppendChild(objNewRoot);

            doc.Save(savePath);
        } // End Sub ChangeStandalone 


        public static System.Data.Common.DbDataAdapter GetDataAdapter(string strSQL)
        {
            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = System.Environment.MachineName;
            csb.InitialCatalog = "SwissRe_Test_V3";
            csb.IntegratedSecurity = true;
            if (!csb.IntegratedSecurity)
            {
                csb.UserID = "ApertureWebServices";
                csb.Password = "";
            } // End if (!csb.IntegratedSecurity)

            return new System.Data.SqlClient.SqlDataAdapter(strSQL, csb.ConnectionString);
        } // End Function GetDataAdapter


    } // End Class GroupedExport


} // End Namespace NestedHierarchicalXml
