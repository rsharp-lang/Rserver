Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.WebCloud.VBScript

<Package("vbhtml")>
Module VBHtmlTemplate

    <ExportAPI("rendering")>
    Public Function Rendering(file As String, wwwroot As String,
                              <RListObjectArgument>
                              Optional symbols As list = Nothing,
                              Optional env As Environment = Nothing) As Object

        Return vbhtml.ReadHTML(wwwroot, file)
    End Function
End Module
